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
            Label articul = this.FindByName<Label>("articul");
            Label unit = this.FindByName<Label>("unit");

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

        private void ClearFields(Object source, ElapsedEventArgs e)
        { 
            name.Text = " ";
            price.Text = " ";
            articul.Text =  " ";
            barcode.Text = " ";
            unit.Text = " ";
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

                    barcode.Text = Code + " ";
                    if (deserialized.Name.Length >= 26)
                    {
                        name.Text = deserialized.Name.Substring(0, 25) + "... ";
                    }
                    else
                    {
                        name.Text = deserialized.Name + " ";
                    }
                    price.Text = deserialized.Price + " ";
                    articul.Text = deserialized.VendorCode + " ";
                    unit.Text = deserialized.ED + ". ";

                    var timerClearFields = new System.Timers.Timer();
                    timerClearFields.Interval = 5000;

                    timerClearFields.Elapsed += ClearFields;
                    timerClearFields.AutoReset = false;
                    timerClearFields.Enabled = true;
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

            public string VendorCode { get; set; }

            public string Name { get; set; }

            public string Price { get; set; }

            public string ED { get; set; }
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
