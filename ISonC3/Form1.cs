using Microsoft.SolverFoundation.Services;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISonC3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            GridView1.RowCount = 3;
            GridView1.ColumnCount = 10;
            GridView1.AllowUserToAddRows = false;
            GridView2.RowCount = 3;
            GridView2.ColumnCount = 10;
            GridView2.AllowUserToAddRows = false;
            GridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            GridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
           
        }
        List<XY> coords = new List<XY>();
        TypeOfFunction[] func { get; set; } = new TypeOfFunction[] {
                new TypeOfFunction("Линейная",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            exp.Append($"({coord.Y} - ( A * {coord.X} + B ))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                    (a,b,x)=> a*x+b
                ),
                new TypeOfFunction("Показательная",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            if(coord.X == 0)
                            {
                                exp.Append($"({coord.Y} - ( A *(B^(0.0001))))^2");
                            }
                            else
                            exp.Append($"({coord.Y} - ( A *(B^({coord.X}))))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                    (a, b, x) => a* Math.Pow(Math.Abs(b), x)
                ),
                new TypeOfFunction("Дробная",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            exp.Append($"({coord.Y}-1/( A * {coord.X} + B ))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                      (a, b, x) =>
                      {
                          try
                          {
                            return 1 /(a * x + b);
                          }
                          catch
                          {
                              return double.PositiveInfinity;
                          }
                      }

                    ),
                new TypeOfFunction("логарифмическая",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            exp.Append($"({coord.Y}-( A * {Math.Log(Math.Abs(coord.X))} + B ))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                    (a, b, x) => a*Math.Log(Math.Abs(x)) + b
                ),
                new TypeOfFunction("Степенная",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            exp.Append($"({coord.Y}-( A * {coord.X}^B ))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                    (a, b, x) => a*Math.Pow(Math.Abs(x),b)
                ),
                new TypeOfFunction("Гиперболическая",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            if(coord.X == 0)
                            {
                                exp.Append($"({coord.Y}-( A + B / 0.0000000001 ))^2");
                            }
                            else
                            exp.Append($"({coord.Y}-( A + B / {coord.X}))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                    (a, b, x) =>
                    {
                        if(x == 0)
                        {
                            if(b > 0)
                                return double.PositiveInfinity;
                            if(b == 0)
                                return a;
                            else
                            {
                                return double.NegativeInfinity;
                            }
                        }
                        return a + b/x;
                    }
                ),
                new TypeOfFunction("Дробно-рациональная",
                    (coords) =>
                    {   StringBuilder exp = new StringBuilder();
                        foreach(var coord in coords)
                        {
                            exp.Append($"({coord.Y}-({coord.X}/( A * {coord.X} + B) ))^2");
                            if (!coord.Equals(coords.Last()))
                            {
                                exp.Append(" + ");
                            }
                        }
                        return exp.ToString();
                    },
                    (a, b, x) => x/(a*x+b)
                ),};
        private void Button1_Click(object sender, EventArgs e)
        {
            coords.Clear();
            //try
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        coords.Add(new XY(Convert.ToDouble(GridView1.Rows[0].Cells[j].Value),
            //            Convert.ToDouble(GridView1.Rows[1].Cells[j].Value)));
            //    }
            //}
            //catch
            //{
            //    MessageBox.Show("Данные введены неправильно!");
            //    return;
            //}
            //for tests
            coords = new List<XY>()
            {
                new XY(-4, 0.04),
                new XY(-3,0.06),
                new XY(-2,0.23),
                new XY(-1, 0.3),
                new XY(0, 0.57),
                new XY(1,0.99),
                new XY(2,2.34),
                new XY(3,3.79),

                new XY(4,8),
                new XY(5,15.9)
            };
            coords = (from xy in coords
                      orderby xy.X
                      select xy).ToList();
            (double, double, double) xprop =
                calcProperties(coords.FirstOrDefault().X, coords.LastOrDefault().X);
            (double, double, double) YbyXprop =
                newYs(xprop);
            (double, double, double) yprop =
                calcProperties(coords.FirstOrDefault().Y, coords.LastOrDefault().Y);
            func[0].Value = Math.Abs(YbyXprop.Item1 - yprop.Item1);  //линейная
            func[1].Value = Math.Abs(YbyXprop.Item1 - yprop.Item2); //показательная
            func[2].Value = Math.Abs(YbyXprop.Item1 - yprop.Item3);  //дробная
            func[3].Value = Math.Abs(YbyXprop.Item2 - yprop.Item1);  //логарифмическая
            func[4].Value = Math.Abs(YbyXprop.Item2 - yprop.Item2); //степенная
            func[5].Value = Math.Abs(YbyXprop.Item3 - yprop.Item1);  //гиперболическая
            func[6].Value = Math.Abs(YbyXprop.Item3 - yprop.Item2);
            TypeOfFunction foundFunc = (from f in func orderby f.Value select f).First();
            (double, double) abItems = solveFunction(foundFunc);
            double a = abItems.Item1;
            double b = abItems.Item2;
            XY[] arraycoords = (from xy in coords orderby xy.X select xy).ToArray();
            
            try
            {
                LineSeries lS = new LineSeries();
                var myModel = new PlotModel { Title = "График" };
                FunctionSeries functionSeries = new FunctionSeries((x) => foundFunc.Func(a, b, x), coords.FirstOrDefault().X, coords.LastOrDefault().X, 0.0001, "function");
                for (int j = 0; j < 10; j++)
                {
                    GridView2.Rows[0].Cells[j].Value = arraycoords[j].X;
                    GridView2.Rows[1].Cells[j].Value = Math.Round(foundFunc.Func(a, b, arraycoords[j].X), 3);
                    lS.Points.Add(new DataPoint(arraycoords[j].X, arraycoords[j].Y));   
                }
                textBox1.Text = foundFunc.Name;
                textBox2.Text = Math.Round(a, 3).ToString();
                textBox3.Text = Math.Round(b, 3).ToString();
                lS.Title = "Исходные данные";
                myModel.Series.Add(functionSeries);
                myModel.Series.Add(lS);
                plotView1.Model = myModel;
            }
            catch
            {
                MessageBox.Show("fatal error, try again");
            }
            

        }
        (double, double) solveFunction(TypeOfFunction func)
        {
            SolverContext context = SolverContext.GetContext();
            context.ClearModel();
            Microsoft.SolverFoundation.Services.Model model = context.CreateModel();
            var decisionA = new Decision(Domain.Real, "A");
            var decisionB = new Decision(Domain.Real, "B");
            model.AddDecision(decisionA);
            model.AddDecision(decisionB);
            model.AddGoal("Goal", GoalKind.Minimize, func.GetFuncString(coords));
            context.Solve();
            return (decisionA.GetDouble(), decisionB.GetDouble());
        }
        (double,double,double) calcProperties(double c0, double c1)
        {
            double first = (c0 + c1) / 2;
            double second = Math.Sqrt(Math.Abs(c0 * c1));
            double third = (2 * c0 * c1) / (c0 + c1);
            return (first, second, third);
        }
        (double, double, double) newYs((double, double, double) xprop)
        {
            return (getYbyX(xprop.Item1),
                getYbyX(xprop.Item2),
                getYbyX(xprop.Item3));
        }
        (XY?, XY) defCoords(double x)
        {
            XY? temp = null;
            foreach (XY coord in coords)
            {
                if (coord.X >= x)
                {
                    return (temp, coord);
                }
                temp = coord;
            }
            return (null, coords.LastOrDefault());
        }
        double calcY(XY first, XY second, double x)
        {
            double y =
                ((first.X * second.Y - second.X * first.Y) + (first.Y - second.Y) * x) /
                (first.X - second.X);
            return y;
        }
        double getYbyX(double x)
        {
            (XY?, XY) coords = defCoords(x);
            if (coords.Item1 != null)
                return calcY(coords.Item1.Value,
                    coords.Item2,
                    x);
            return coords.Item2.Y;
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
