using System;
using System.Collections.Generic;

public class ExternalMarket {
    public readonly Dictionary<HoneyType, Tuple<double, double>> priceBounds;
    public readonly Dictionary<HoneyType, double> currentPrices;

    private static readonly double sensitivity = 0.25;

    public ExternalMarket(Dictionary<HoneyType, double> basePrices, Dictionary<HoneyType, Tuple<double, double>> priceBounds) {
        this.currentPrices = new(basePrices);
        this.priceBounds = new(priceBounds);
    }

}
