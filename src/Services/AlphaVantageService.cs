﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Finance.Net.Enums;
using Finance.Net.Exceptions;
using Finance.Net.Interfaces;
using Finance.Net.Models.AlphaVantage;
using Finance.Net.Models.AlphaVantage.Dtos;
using Finance.Net.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;

namespace Finance.Net.Services;
/// <inheritdoc />
public class AlphaVantageService(ILogger<AlphaVantageService> logger,
IHttpClientFactory httpClientFactory,
IOptions<FinanceNetConfiguration> options,
IReadOnlyPolicyRegistry<string> policyRegistry) : IAlphaVantageService
{
    private readonly ILogger<AlphaVantageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly FinanceNetConfiguration _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly AsyncPolicy _retryPolicy = policyRegistry?.Get<AsyncPolicy>(Constants.DefaultHttpRetryPolicy) ?? throw new ArgumentNullException(nameof(policyRegistry));

    /// <inheritdoc />
    public async Task<InstrumentOverview?> GetOverviewAsync(string symbol, CancellationToken token = default)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.AlphaVantageHttpClientName);
        var url = Constants.AlphaVantageApiUrl + "/query?function=OVERVIEW" +
            $"&symbol={symbol}" +
            $"&apikey={_options.AlphaVantageApiKey}";

        try
        {
            var instrumentOverview = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var httpResponse = await httpClient.GetAsync(url, token).ConfigureAwait(false);
                    httpResponse.EnsureSuccessStatusCode();

                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                    if (jsonResponse.Contains(Constants.ResponseApiLimitExceeded))
                    {
                        throw new FinanceNetException($"{Constants.ResponseApiLimitExceeded} for {symbol}");
                    }
                    else
                    {
                        var overview = JsonConvert.DeserializeObject<InstrumentOverview>(jsonResponse);
                        var isNullObj = Helper.AreAllPropertiesNull(overview);
                        return isNullObj ? throw new FinanceNetException(Constants.ValidationMsgAllFieldsEmpty) : overview;
                    }
                });
            return instrumentOverview;
        }
        catch (Exception ex)
        {
            throw new FinanceNetException($"No overview found for {symbol}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Record>> GetRecordsAsync(string symbol, DateTime? startDate = null, DateTime? endDate = null, CancellationToken token = default)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.AlphaVantageHttpClientName);
        var url = Constants.AlphaVantageApiUrl + "/query?function=TIME_SERIES_DAILY_ADJUSTED" +
            $"&symbol={symbol}&outputsize=full&apikey={_options.AlphaVantageApiKey}";
        Guard.Against.NullOrEmpty(symbol);

        startDate ??= DateTime.UtcNow.AddDays(-7).Date;
        endDate ??= DateTime.UtcNow;
        if (startDate > endDate)
        {
            throw new FinanceNetException("startDate earlier than endDate");
        }
        if (endDate.Value.Date >= DateTime.UtcNow.Date)
        {
            endDate = DateTime.UtcNow.Date;
        }
        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var jsonResponse = await Helper.FetchJsonDocumentAsync(httpClient, _logger, url, token);
                if (jsonResponse.Contains(Constants.ResponseApiLimitExceeded))
                {
                    throw new FinanceNetException($"{Constants.ResponseApiLimitExceeded} for {symbol}");
                }
                var result = ParseHistoryRecords(symbol, startDate, endDate, jsonResponse);
                return result.IsNullOrEmpty() ? throw new FinanceNetException(Constants.ValidationMsgAllFieldsEmpty) : result;
            });
        }
        catch (Exception ex)
        {
            throw new FinanceNetException($"No Record found for {symbol}", ex);
        }
    }

    private List<Record> ParseHistoryRecords(string symbol, DateTime? startDate, DateTime? endDate, string jsonResponse)
    {
        var result = new List<Record>();
        Guard.Against.Null(startDate);
        var data = JsonConvert.DeserializeObject<DailyRecordRoot>(jsonResponse);
        if (data?.TimeSeries == null)
        {
            throw new FinanceNetException("data is invalid");
        }
        foreach (var item in data.TimeSeries)
        {
            var today = item.Key.Date;
            if (today > endDate || today < startDate)
            {
                continue;
            }
            if (result.Any(e => e.Date == today))
            {
                _logger.LogWarning("Bug: Course for {Symbol} for {Date} already added!", symbol, today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else
            {
                result.Add(new Record
                {
                    Date = today,
                    Open = item.Value.Open,
                    Low = item.Value.Low,
                    High = item.Value.High,
                    Close = item.Value.AdjustedClose,
                    AdjustedClose = item.Value.AdjustedClose,
                    Volume = item.Value.Volume,
                    SplitCoefficient = item.Value.SplitCoefficient,
                });
            }
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IntradayRecord>> GetIntradayRecordsAsync(string symbol, DateTime startDate, DateTime? endDate = null, EInterval interval = EInterval.Interval_15Min, CancellationToken token = default)
    {
        Guard.Against.NullOrEmpty(symbol);
        var result = new List<IntradayRecord>();
        if (startDate > endDate)
        {
            throw new FinanceNetException("startDate earlier than endDate");
        }
        endDate ??= DateTime.UtcNow.Date;
        if (endDate.Value.Date >= DateTime.UtcNow.Date)
        {
            endDate = DateTime.UtcNow.Date;
        }

        for (var currentMonth = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, DateTimeKind.Utc); currentMonth <= endDate; currentMonth = currentMonth.AddMonths(1))
        {
            if (currentMonth == endDate &&
                ((endDate.Value.Day == 1 && endDate.Value.DayOfWeek == DayOfWeek.Saturday) ||
                    (endDate.Value.Day == 1 && endDate.Value.DayOfWeek == DayOfWeek.Sunday) ||
                    (endDate.Value.Day == 2 && endDate.Value.DayOfWeek == DayOfWeek.Sunday)))
            {
                // dont query for data which not exists (api exception)
                break;
            }
            var currentCourses = await GetIntradayRecordsByMonthAsync(symbol, currentMonth, interval, token);
            result.AddRange(currentCourses);
        }
        result = result.Where(e => e.DateTime >= startDate).ToList();
        return result.IsNullOrEmpty() ? throw new FinanceNetException(Constants.ValidationMsgAllFieldsEmpty) : result;
    }

    private async Task<List<IntradayRecord>> GetIntradayRecordsByMonthAsync(string symbol, DateTime month, EInterval interval, CancellationToken token = default)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.AlphaVantageHttpClientName);
        var url = Constants.AlphaVantageApiUrl + "/query?function=TIME_SERIES_INTRADAY" +
            $"&symbol={symbol}" +
            $"&interval={interval.GetDescription()}" +
            $"&month={month:yyyy-MM}" +
            "&extended_hours=false" +      // no pre and post market trading
            "&outputsize=full" +
            $"&apikey={_options.AlphaVantageApiKey}";

        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.GetAsync(url, token).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                if (jsonResponse.Contains(Constants.ResponseApiLimitExceeded))
                {
                    throw new FinanceNetException($"{Constants.ResponseApiLimitExceeded} for {symbol}");
                }
                var result = ParseIntradayRecords(symbol, interval, jsonResponse);
                return result.IsNullOrEmpty() ? throw new FinanceNetException(Constants.ValidationMsgAllFieldsEmpty) : result;
            });
        }
        catch (Exception ex)
        {
            throw new FinanceNetException($"No intraday record found for {symbol}", ex);
        }
    }

    private static List<IntradayRecord> ParseIntradayRecords(string symbol, EInterval interval, string jsonResponse)
    {
        var result = new List<IntradayRecord>();
        var data = JsonConvert.DeserializeObject<IntradayRecordRoot>(jsonResponse);

        var timeseries = interval switch
        {
            EInterval.Interval_1Min => data?.TimeSeries1Min ?? throw new FinanceNetException($"no timeSeries for {symbol}"),
            EInterval.Interval_5Min => data?.TimeSeries5Min ?? throw new FinanceNetException($"no timeSeries for {symbol}"),
            EInterval.Interval_15Min => data?.TimeSeries15Min ?? throw new FinanceNetException($"no timeSeries for {symbol}"),
            EInterval.Interval_30Min => data?.TimeSeries30Min ?? throw new FinanceNetException($"no timeSeries for {symbol}"),
            EInterval.Interval_60Min => data?.TimeSeries60Min ?? throw new FinanceNetException($"no timeSeries for {symbol}"),
            _ => throw new NotImplementedException(),
        };
        foreach (var item in timeseries)
        {
            var dateTime = DateTime.ParseExact(item.Key, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            result.Add(new IntradayRecord
            {
                DateTime = dateTime,

                Open = item.Value.Open,
                Low = item.Value.Low,
                High = item.Value.High,
                Close = item.Value.Close,
                Volume = item.Value.Volume,
            });
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ForexRecord>> GetForexRecordsAsync(string currency1, string currency2, DateTime startDate, DateTime? endDate = null, CancellationToken token = default)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.AlphaVantageHttpClientName);
        Guard.Against.NullOrEmpty(currency1);
        Guard.Against.NullOrEmpty(currency2);
        if (startDate > endDate)
        {
            throw new FinanceNetException("startDate earlier than endDate");
        }

        endDate ??= DateTime.UtcNow.Date;
        if (endDate.Value.Date >= DateTime.UtcNow.Date)
        {
            endDate = DateTime.UtcNow.Date;
        }
        var url = Constants.AlphaVantageApiUrl + "/query?function=FX_DAILY" +
            $"&from_symbol={currency1}&to_symbol={currency2}&outputsize=full&apikey={_options.AlphaVantageApiKey}";

        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var jsonResponse = await Helper.FetchJsonDocumentAsync(httpClient, _logger, url, token);
                if (jsonResponse.Contains(Constants.ResponseApiLimitExceeded))
                {
                    throw new FinanceNetException($"{Constants.ResponseApiLimitExceeded} for {currency1} /{currency2}");
                }
                var result = ParseHistoryForexRecords(currency1, currency2, startDate, endDate, jsonResponse);
                return result.IsNullOrEmpty() ? throw new FinanceNetException(Constants.ValidationMsgAllFieldsEmpty) : result;
            });
        }
        catch (Exception ex)
        {
            throw new FinanceNetException($"No forex record found for {currency1}, {currency2}", ex);
        }
    }

    private List<ForexRecord> ParseHistoryForexRecords(string currency1, string currency2, DateTime startDate, DateTime? endDate, string jsonResponse)
    {
        var result = new List<ForexRecord>();
        var data = JsonConvert.DeserializeObject<DailyForexRecordRoot>(jsonResponse);
        if (data?.TimeSeries == null)
        {
            throw new FinanceNetException("data is invalid");
        }
        foreach (var item in data.TimeSeries)
        {
            var today = item.Key;
            if (today > endDate || today < startDate)
            {
                continue;
            }
            if (result.Any(e => e.Date == today))
            {
                _logger.LogWarning("Bug: {Currency1} /{Currency2} for {Date} already added!", currency1, currency2, today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else
            {
                result.Add(new ForexRecord
                {
                    Date = item.Key,
                    Open = item.Value.Open,
                    Low = item.Value.Low,
                    High = item.Value.High,
                    Close = item.Value.Close,
                });
            }
        }
        return result;
    }
}
