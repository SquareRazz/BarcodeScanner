using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;


namespace BarcodeScanner
{
    
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public Timer timer = new Timer();
        public Timer timerFocus = new Timer();
        string nameString = "";
        string priceString = "";
        string vendorCodeString = "";
        string barCodeString = "";
        string EDString = "";

        public static string Code { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            SetFocus();
        }

        private void ClearFieldsEvent(Object source, ElapsedEventArgs e)
        {
            barcode.Text = "";
            barcode2.Text = "";
            name.Text = "";
            price.Text = "";
            articul.Text = "";
            unit.Text = "";
        }

        public void SetFocus()
        {
            EnteredBarcode.Focus();
        }

        public MainPage()
        {
            InitializeComponent();

            EnteredBarcode.Focus();

            timer.Interval = 5000;
            timer.Elapsed += ClearFieldsEvent;
            timer.AutoReset = false;
            timer.Enabled = true;

            timerFocus = new Timer
            {
                Interval = 3500
            };

            timerFocus.Elapsed += OnTimedEvent;
            timerFocus.AutoReset = true;
            timerFocus.Enabled = true;
        }

        private async void Search_Click(object sender, EventArgs e)
        {
            Code = EnteredBarcode.Text;

            EnteredBarcode.Text = "";

            using (HttpClient client = new HttpClient())
            {
                    HttpResponseMessage response = await client.GetAsync("http://10.7.110.4:81/PotamusProduct?article=" + Code);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    var deserialized = JsonConvert.DeserializeObject<PotamusProduct>(responseBody);

                    if(deserialized.Name.Length >= 26)
                    {
                        nameString = deserialized.Name.Substring(0, 25) + "... ";
                    }
                    else
                    {
                        nameString = deserialized.Name + " ";
                    }
                    priceString = deserialized.Price + " ";
                    vendorCodeString = deserialized.VendorCode + " ";
                    barCodeString = deserialized.BarCode;
                    EDString = deserialized.ED + ". ";
            }

            name.Text = nameString;
            price.Text = priceString;
            articul.Text = vendorCodeString;
            barcode.Text = barCodeString;
            barcode2.Text = barCodeString;
            unit.Text = EDString;

            timer.Start();

            EnteredBarcode.Focus();
        }

        public class PotamusProduct
        {
            public string BarCode { get; set; }

            public string VendorCode { get; set; }

            public string Name { get; set; }

            public string Price { get; set; }

            public string Emiter { get; set; }

            public string Reference { get; set; }

            public string Place { get; set; }

            public string GoodId { get; set; }

            public string ED { get; set; }

            public string Rest { get; set; }

            public string Reserve { get; set; }

            public string BonusQTY { get; set; }
        }

        private void EnteredBarcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 13)
            {
                Search_Click(sender, e);
            }
        }
    }
}


//public static string connectionString = "Data Source=10.7.131.203;Initial Catalog=gladiator;Persist Security Info=True;User ID=mob_term;Password=54321";

//private static DataTable ExecuteCommand(SqlCommand command, List<SqlParameter> parameters)
//{
//    DataTable queryData = new DataTable();

//    try
//    {
//        SqlConnection connect = new SqlConnection(connectionString);
//        SqlDataAdapter adapter = new SqlDataAdapter();
//        SqlCommand sqlComand = command;
//        sqlComand.CommandType = CommandType.StoredProcedure;
//        sqlComand.Connection = connect;

//        foreach (var parameter in parameters)
//        {
//            sqlComand.Parameters.Add(parameter);
//        }

//        adapter.SelectCommand = sqlComand;
//        adapter.Fill(queryData);
//        connect.Close();

//    }
//    catch { }


//    return queryData;
//}


//// вызываем процедуру на сервере Potamus для получения информации о товаре по шкоду или артикулу
//var sqlComand = new SqlCommand("dbo.I_CheckTovar");

//var sqlParameters = new List<SqlParameter>();

//sqlParameters.Add(new SqlParameter("@p_TovarID", Code));

//if (Code.Trim().Length == 8)
//{
//    sqlParameters.Add(new SqlParameter("@pis_articul", "1"));
//}
//else
//{
//    sqlParameters.Add(new SqlParameter("@pis_articul", "0"));
//}

//var result = ExecuteCommand(sqlComand, sqlParameters);

//if (result.Rows.Count > 0 && result.Rows[0][0].ToString() != "-1")
//{
//    // формирование товара
//    PotamusProduct product = new PotamusProduct
//    {
//        BarCode = result.Rows[0]["barcode"].ToString(),
//        VendorCode = result.Rows[0]["articul"].ToString(),
//        Name = result.Rows[0]["name"].ToString(),
//        Price = result.Rows[0]["sell_price_nds"].ToString(),
//        Emiter = result.Rows[0]["producer"].ToString(),

//        Reference = result.Rows[0]["reference"].ToString(),
//        Place = result.Rows[0]["place"].ToString(),
//        GoodId = result.Rows[0]["good_id"].ToString(),
//        ED = result.Rows[0]["ed"].ToString(),
//        Rest = result.Rows[0]["rest"].ToString(),
//        Reserve = result.Rows[0]["reserve"].ToString(),
//        BonusQTY = result.Rows[0]["bonus_qty"].ToString()
//    };

//    Label barcode = this.FindByName<Label>("barcode");
//    Label name = this.FindByName<Label>("name");
//    Label price = this.FindByName<Label>("price");

//    barcode.Text = Code;
//    name.Text = product.Name;
//    price.Text = product.Price;
//}