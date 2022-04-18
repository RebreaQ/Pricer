using System;
using LinearInterpolation;
using System.Collections.Generic;


namespace Discount{
    struct DateVal{
        public DateTime date;
        public double val;
        public double getDoubleDate(){
            return (double)date.Ticks;
        }
        public DateVal(DateTime x, double y){
            date = x;
            val = y;
        }
    }
    static class Reader{
        private static DateTime StrToDate(String str){
            DateTime Now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (str == "ON"){
                return Now;
            }
            Char lastSym = str[str.Length - 1];
            str = str.Remove(str.Length - 1);
            if (lastSym == 'W'){
                Now = Now.AddDays(7 * Convert.ToInt32(str));
            }
            else if (lastSym == 'M'){
                Now = Now.AddMonths(Convert.ToInt32(str));
            }
            else if (lastSym == 'Y'){
                Now = Now.AddYears(Convert.ToInt32(str));
            }
            else{
                throw new Exception("Error! Invalid input argument!");
            }
            return Now;
        }
        public static DateVal[] ReadFile(String path){
            List <DateVal> res = new List<DateVal>();
            using(var reader = new System.IO.StreamReader(path)){
                DateVal currentVal;
                reader.ReadLine();
                while (!reader.EndOfStream){
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    currentVal.date = StrToDate(values[0]);
                    currentVal.val = (double)Convert.ToInt32(values[1]);
                    res.Add(currentVal);
                }
            }
            return res.ToArray();
        }
    }
    static class RubleDiscount
    {
        
        public static DateVal[] GetRubleDiscountCurve(DateVal[] factSwapPoints, 
        Func <DateTime[], DateVal[]> getDollarDiscount,
        DateTime[] reqDates,
        double spotPrice){
            DateVal[] requiredRubbleDiscount = new DateVal[reqDates.Length];
            Array.Sort(factSwapPoints, delegate(DateVal DateVal1, DateVal DateVal2){
                                                    return Convert.ToInt32(
                                                        DateVal1.date > DateVal2.date);
                                                });
            // get fact dollar discount factors
            DateTime[] dollarDFTimes = new DateTime[factSwapPoints.Length];
            for (int i = 0; i < dollarDFTimes.Length; ++i){
                dollarDFTimes[i] = factSwapPoints[i].date;
            }
            DateVal[] factDollarDF = getDollarDiscount(dollarDFTimes);
            // get fact rubble discount factors
            Point[] factDollarDFPoint = new Point[factSwapPoints.Length];
            Point[] factRubleDFPoint = new Point[factSwapPoints.Length];
            for (int i = 0; i < factRubleDFPoint.Length; ++i){
                factRubleDFPoint[i].x = factDollarDF[i].getDoubleDate();
                factRubleDFPoint[i].y = factDollarDF[i].val / 
                    (factSwapPoints[i].val / (1e4 * spotPrice) + 1);
                factDollarDFPoint[i].x = factDollarDF[i].getDoubleDate();
                factDollarDFPoint[i].y = factDollarDF[i].val;
            }
            var rubbleDF = LinearInterpolation
                .Interpolation.GetInterpolation(factRubleDFPoint);
            var DollarDF = LinearInterpolation
                .Interpolation.GetInterpolation(factDollarDFPoint);

            
            for (int i = 0; i < reqDates.Length; ++i){
                requiredRubbleDiscount[i].date = reqDates[i];
                requiredRubbleDiscount[i].val = rubbleDF(requiredRubbleDiscount[i].getDoubleDate());
            }
            return requiredRubbleDiscount;
        } 
        public static DateVal[] GetRubleSwapCurve(DateVal[] factSwapPoints, 
        Func <DateTime[], DateVal[]> getDollarDiscount,
        DateTime[] reqDates,
        double spotPrice){
            DateVal[] requiredSwapPoints = new DateVal[reqDates.Length];
            DateVal[] rubleDF = GetRubleDiscountCurve(factSwapPoints, getDollarDiscount, 
                reqDates, spotPrice);
            DateVal[] dollarDF = getDollarDiscount(reqDates);
            for (int i = 0; i < requiredSwapPoints.Length;  ++i){
                requiredSwapPoints[i].date = reqDates[i];
                requiredSwapPoints[i].val = 1e4 * spotPrice * 
                    (dollarDF[i].val / rubleDF[i].val - 1);
            }
            return requiredSwapPoints;
        } 
 
    };
}