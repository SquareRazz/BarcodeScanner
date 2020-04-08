using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        public static string Code { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Entry entry = this.FindByName<Entry>("EnteredBarcode");
            Label barcode = this.FindByName<Label>("barcode");
            Label name = this.FindByName<Label>("name");
            Label price = this.FindByName<Label>("price");

            entry.Focus();

            var timer = new System.Timers.Timer();
            timer.Interval = 3500;

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            SetFocus();
        }

        public void SetFocus()
        {
            Entry entry = this.FindByName<Entry>("EnteredBarcode");

            entry.Focus();

        }

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Search_Click(object sender, EventArgs e)
        {
            Code = this.EnteredBarcode.Text;

            this.EnteredBarcode.Text = "";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://10.7.110.4:81/PotamusProduct?article=" + Code);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    var deserialized = JsonConvert.DeserializeObject<PotamusProduct>(responseBody);

                    barcode.Text = "Штрих-код: " + Code;
                    name.Text = "Назва: " + deserialized.Name;
                    price.Text = "Ціна: " + deserialized.Price + " ₴";
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", ex.Message);
                }
            }

            this.EnteredBarcode.Focus();
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