﻿using System.Diagnostics.CodeAnalysis;
using CsvHelper.Configuration;
using DotNetFinance.Models.Xetra.Dto;

namespace DotNetFinance.Mappings;

[ExcludeFromCodeCoverage]
public class XetraInstrumentsMapping : ClassMap<InstrumentItem>
{
	public XetraInstrumentsMapping()
	{
		Map(m => m.ProductStatus).Name("Product Status");
		Map(m => m.InstrumentStatus).Name("Instrument Status");
		Map(m => m.Instrument).Name("Instrument");
		Map(m => m.ISIN).Name("ISIN");
		Map(m => m.ProductID).Name("Product ID");
		Map(m => m.InstrumentID).Name("Instrument ID");
		Map(m => m.WKN).Name("WKN");
		Map(m => m.Mnemonic).Name("Mnemonic");
		Map(m => m.MICCode).Name("MIC Code");
		Map(m => m.CCPeligibleCode).Name("CCP eligible Code");
		Map(m => m.TradingModelType).Name("Trading Model Type");
		Map(m => m.ProductAssignmentGroup).Name("Product Assignment Group");
		Map(m => m.ProductAssignmentGroupDescription).Name("Product Assignment Group Description");
		Map(m => m.DesignatedSponsorMemberID).Name("Designated Sponsor Member ID");
		Map(m => m.DesignatedSponsor).Name("Designated Sponsor");
		Map(m => m.PriceRangeValue).Name("Price Range Value");
		Map(m => m.PriceRangePercentage).Name("Price Range Percentage");
		Map(m => m.MinimumQuoteSize).Name("Minimum Quote Size");
		Map(m => m.InstrumentType).Name("Instrument Type");
		Map(m => m.TickSize1).Name("Tick Size 1");
		Map(m => m.UpperPriceLimitMax).Name("Upper Price Limit Max");
		Map(m => m.TickSize2).Name("Tick Size 2");
		Map(m => m.UpperPriceLimit2).Name("Upper Price Limit 2");
		Map(m => m.TickSize3).Name("Tick Size 3");
		Map(m => m.UpperPriceLimit3).Name("Upper Price Limit 3");
		Map(m => m.TickSize4).Name("Tick Size 4");
		Map(m => m.UpperPriceLimit4).Name("Upper Price Limit 4");
		Map(m => m.TickSize5).Name("Tick Size 5");
		Map(m => m.UpperPriceLimit5).Name("Upper Price Limit 5");
		Map(m => m.TickSize6).Name("Tick Size 6");
		Map(m => m.UpperPriceLimit6).Name("Upper Price Limit 6");
		Map(m => m.TickSize7).Name("Tick Size 7");
		Map(m => m.UpperPriceLimit7).Name("Upper Price Limit 7");
		Map(m => m.TickSize8).Name("Tick Size 8");
		Map(m => m.UpperPriceLimit8).Name("Upper Price Limit 8");
		Map(m => m.TickSize9).Name("Tick Size 9");
		Map(m => m.UpperPriceLimit9).Name("Upper Price Limit 9");
		Map(m => m.TickSize10).Name("Tick Size 10");
		Map(m => m.UpperPriceLimit10).Name("Upper Price Limit 10");
		Map(m => m.TickSize11).Name("Tick Size 11");
		Map(m => m.UpperPriceLimit11).Name("Upper Price Limit 11");
		Map(m => m.TickSize12).Name("Tick Size 12");
		Map(m => m.UpperPriceLimit12).Name("Upper Price Limit 12");
		Map(m => m.TickSize13).Name("Tick Size 13");
		Map(m => m.UpperPriceLimit13).Name("Upper Price Limit 13");
		Map(m => m.TickSize14).Name("Tick Size 14");
		Map(m => m.UpperPriceLimit14).Name("Upper Price Limit 14");
		Map(m => m.TickSize15).Name("Tick Size 15");
		Map(m => m.UpperPriceLimit15).Name("Upper Price Limit 15");
		Map(m => m.TickSize16).Name("Tick Size 16");
		Map(m => m.UpperPriceLimit16).Name("Upper Price Limit 16");
		Map(m => m.TickSize17).Name("Tick Size 17");
		Map(m => m.UpperPriceLimit17).Name("Upper Price Limit 17");
		Map(m => m.TickSize18).Name("Tick Size 18");
		Map(m => m.UpperPriceLimit18).Name("Upper Price Limit 18");
		Map(m => m.TickSize19).Name("Tick Size 19");
		Map(m => m.UpperPriceLimit19).Name("Upper Price Limit 19");
		Map(m => m.TickSize20).Name("Tick Size 20");
		Map(m => m.UpperPriceLimit20).Name("Upper Price Limit 20");
		Map(m => m.NumberofDecimalDigits).Name("Number of Decimal Digits");
		Map(m => m.UnitofQuotation).Name("Unit of Quotation");
		Map(m => m.MarketSegment).Name("Market Segment");
		Map(m => m.MarketSegmentSupplement).Name("Market Segment Supplement");
		Map(m => m.ClearingLocation).Name("Clearing Location");
		Map(m => m.PrimaryMarketMICCode).Name("Primary Market MIC Code");
		Map(m => m.ReportingMarket).Name("Reporting Market");
		Map(m => m.SettlementPeriod).Name("Settlement Period");
		Map(m => m.SettlementCurrency).Name("Settlement Currency");
		Map(m => m.ClosedBookIndicator).Name("Closed Book Indicator");
		Map(m => m.MarketImbalanceIndicator).Name("Market Imbalance Indicator");
		Map(m => m.CUMEXIndicator).Name("CUM/EX Indicator");
		Map(m => m.MinimumIcebergTotalVolume).Name("Minimum Iceberg Total Volume");
		Map(m => m.MinimumIcebergDisplayVolume).Name("Minimum Iceberg Display Volume");
		Map(m => m.EMDIIncrementalA_Unnetted).Name("EMDI Incremental A - Unnetted");
		Map(m => m.EMDIIncrementalA_UnnettedPort).Name("EMDI Incremental A - Unnetted Port");
		Map(m => m.EMDIIncrementalB_Unnetted).Name("EMDI Incremental B - Unnetted");
		Map(m => m.EMDIIncrementalB_UnnettedPort).Name("EMDI Incremental B - Unnetted Port");
		Map(m => m.EMDISnapshotA_Unnetted).Name("EMDI Snapshot A - Unnetted");
		Map(m => m.EMDISnapshotA_UnnettedPort).Name("EMDI Snapshot A - Unnetted Port");
		Map(m => m.EMDISnapshotB_Unnetted).Name("EMDI Snapshot B - Unnetted");
		Map(m => m.EMDISnapshotB_UnnettedPort).Name("EMDI Snapshot B - Unnetted Port");
		Map(m => m.EMDIMarketDepth_Unnetted).Name("EMDI Market Depth - Unnetted");
		Map(m => m.EMDISnapshotRecoveryTimeInterval_Unnetted).Name("EMDI Snapshot Recovery Time Interval - Unnetted");
		Map(m => m.MDIAddressA_Netted).Name("MDI Address A - Netted");
		Map(m => m.MDIPortA_Netted).Name("MDI Port A - Netted");
		Map(m => m.MDIAddressB_Netted).Name("MDI Address B - Netted");
		Map(m => m.MDIPortB_Netted).Name("MDI Port B - Netted");
		Map(m => m.MDIMarketDepth_Netted).Name("MDI Market Depth - Netted");
		Map(m => m.MDIMarketDepthTimeInterval_Netted).Name("MDI Market Depth Time Interval - Netted");
		Map(m => m.MDIRecoveryTimeInterval_Netted).Name("MDI Recovery Time Interval - Netted");
		Map(m => m.EOBIIncrementalA).Name("EOBI Incremental A");
		Map(m => m.EOBIIncrementalPortA).Name("EOBI Incremental Port A");
		Map(m => m.EOBIIncrementalB).Name("EOBI Incremental B");
		Map(m => m.EOBIIncrementalPortB).Name("EOBI Incremental Port B");
		Map(m => m.EOBISnapshotA).Name("EOBI Snapshot A");
		Map(m => m.EOBISnapshotPortA).Name("EOBI Snapshot Port A");
		Map(m => m.EOBISnapshotB).Name("EOBI Snapshot B");
		Map(m => m.EOBISnapshotPortB).Name("EOBI Snapshot Port B");
		Map(m => m.MarketMakerMemberID).Name("Market Maker Member ID");
		Map(m => m.MarketMaker).Name("Market Maker");
		Map(m => m.RegulatoryLiquidInstrument).Name("Regulatory Liquid Instrument");
		Map(m => m.Pre_tradeLISValue).Name("Pre-trade LIS Value");
		Map(m => m.PartitionID).Name("Partition ID");
		Map(m => m.MultiCCP_eligible).Name("Multi CCP-eligible");
		Map(m => m.TickSizeBand).Name("Tick Size Band");
		Map(m => m.SecuritySubType).Name("Security Sub Type");
		Map(m => m.IssueDate).Name("Issue Date");
		Map(m => m.Underlying).Name("Underlying");
		Map(m => m.MaturityDate).Name("Maturity Date");
		Map(m => m.FlatIndicator).Name("Flat Indicator");
		Map(m => m.CouponRate).Name("Coupon Rate");
		Map(m => m.PreviousCouponPaymentDate).Name("Previous Coupon Payment Date");
		Map(m => m.NextCouponPaymentDate).Name("Next Coupon Payment Date");
		Map(m => m.PoolFactor).Name("Pool Factor");
		Map(m => m.IndexationCoefficient).Name("Indexation Coefficient");
		Map(m => m.AccruedInterestCalculationMethod).Name("Accrued Interest Calculation Method");
		Map(m => m.CountryOfIssue).Name("Country Of Issue");
		Map(m => m.MinimumTradableUnit).Name("Minimum Tradable Unit");
		Map(m => m.In_Subscription).Name("In-Subscription");
		Map(m => m.StrikePrice).Name("Strike Price");
		Map(m => m.MinimumOrderQuantity).Name("Minimum Order Quantity");
		Map(m => m.Off_BookReportingMarket).Name("Off-Book Reporting Market");
		Map(m => m.InstrumentAuctionType).Name("Instrument Auction Type");
		Map(m => m.SpecialistMemberID).Name("Specialist Member ID");
		Map(m => m.Specialist).Name("Specialist");
		Map(m => m.LiquidityProviderUserGroup).Name("Liquidity Provider User Group");
		Map(m => m.SpecialistUserGroup).Name("Specialist User Group");
		Map(m => m.QuotingPeriodStart).Name("Quoting Period Start");
		Map(m => m.QuotingPeriodEnd).Name("Quoting Period End");
		Map(m => m.Currency).Name("Currency");
		Map(m => m.WarrantType).Name("Warrant Type");
		Map(m => m.FirstTradingDate).Name("First Trading Date");
		Map(m => m.LastTradingDate).Name("Last Trading Date");
		Map(m => m.DepositType).Name("Deposit Type");
		Map(m => m.SingleSidedQuoteSupport).Name("Single Sided Quote Support");
		Map(m => m.LiquidityClass).Name("Liquidity Class");
		Map(m => m.CoverIndicator).Name("Cover Indicator");
		Map(m => m.VolatilityCorridorOpeningAuction).Name("VolatilityCorridorOpeningAuction");
		Map(m => m.VolatilityCorridorIntradayAuction).Name("VolatilityCorridorIntradayAuction");
		Map(m => m.VolatilityCorridorClosingAuction).Name("VolatilityCorridorClosingAuction");
		Map(m => m.VolatilityCorridorContinuous).Name("VolatilityCorridorContinuous");
		Map(m => m.DisableOnBookTrading).Name("DisableOnBookTrading");
		Map(m => m.MaximumOrderQuantity).Name("Maximum Order Quantity");
		Map(m => m.MaximumOrderValue).Name("Maximum Order Value");
	}
}
