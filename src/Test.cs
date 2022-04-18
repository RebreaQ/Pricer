using Xunit;
using Point = LinearInterpolation.Point;
using DateVal = Discount.DateVal;
using System.Diagnostics;

namespace Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Point[] points = {new Point(1, 1), new Point(3, 3), new Point(4, 4), new Point(5, 5)};  
            var InterpolatedFunction = LinearInterpolation.Interpolation.GetInterpolation(points);

            Assert.Equal(2, InterpolatedFunction(2));
        }
    }

    public class TestDiscount{
        [Fact]
        public void TestRubleDiscount1(){
            DateVal[] getDollarDiscount(System.DateTime[] d){
                return new DateVal[]{
                    new DateVal(new System.DateTime(2022,01,01), 0.9),
                    new DateVal(new System.DateTime(2024,01,01), 0.8),
                    new DateVal(new System.DateTime(2026,01,01), 0.7)
                    };
            }
            DateVal[] factSwapPoint = new DateVal[]{
                new DateVal(new System.DateTime(2022,01,01), 5000),
                new DateVal(new System.DateTime(2024,01,01), 10000),
                new DateVal(new System.DateTime(2026,01,01), 20000)
            };
            System.DateTime[] reqDate = new System.DateTime[]{
                new System.DateTime(2023,01,01),
                new System.DateTime(2025,01,01),
            };
            double spotPrice = 70.0;
            var ret = Discount.RubleDiscount.GetRubleDiscountCurve(factSwapPoint, 
                getDollarDiscount, reqDate, spotPrice);
            Assert.True(System.Math.Abs(ret[0].val - 0.8411747) < 1e-5);
            Assert.True(System.Math.Abs(ret[1].val - 0.7345699) < 1e-5);
            Assert.Equal(ret[0].date, reqDate[0]);
            Assert.Equal(ret[1].date, reqDate[1]);
        }
        [Fact]
        public void TestSwapCurve1(){
            DateVal[] getDollarDiscount(System.DateTime[] d){
                if (d[0] == new System.DateTime(2022, 01, 01))
                    return new DateVal[]{
                        new DateVal(new System.DateTime(2022,01,01), 0.9),
                        new DateVal(new System.DateTime(2024,01,01), 0.8),
                        new DateVal(new System.DateTime(2026,01,01), 0.7)
                        };
                else
                    return new DateVal[]{
                        new DateVal(new System.DateTime(2023,01,01), 0.85),
                        new DateVal(new System.DateTime(2025,01,01), 0.75),
                        };

            }
            DateVal[] factSwapPoint = new DateVal[]{
                new DateVal(new System.DateTime(2022,01,01), 5000),
                new DateVal(new System.DateTime(2024,01,01), 10000),
                new DateVal(new System.DateTime(2026,01,01), 20000)
            };
            System.DateTime[] reqDate = new System.DateTime[]{
                new System.DateTime(2023,01,01),
                new System.DateTime(2025,01,01),
            };
            double spotPrice = 70.0;
            var ret = Discount.RubleDiscount.GetRubleSwapCurve(factSwapPoint, 
                getDollarDiscount, reqDate, spotPrice);
            Assert.True(System.Math.Abs(ret[0].val - 7344.1396508) < 1e-5);
            Assert.True(System.Math.Abs(ret[1].val - 14703.857276) < 1e-5);
            Assert.Equal(ret[0].date, reqDate[0]);
            Assert.Equal(ret[1].date, reqDate[1]);
        }

        [Fact]
        public void TestReader(){
            System.DateTime now = new System.DateTime(System.DateTime.Now.Year, 
                        System.DateTime.Now.Month, System.DateTime.Now.Day);
            DateVal[] res = Discount.Reader.ReadFile(".//data//RUB swap points.csv");
            Assert.Equal(res[0].date, now);
        }
    }
}