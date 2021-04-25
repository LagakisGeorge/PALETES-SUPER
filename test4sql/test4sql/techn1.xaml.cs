using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Net.Mobile.Forms;
using System.Threading;
using Mono.Data.Sqlite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Plugin.Toast;
using SharpCifs.Smb;
using System.Data.SqlClient;


namespace test4sql
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class techn1 : ContentPage  // timologisi  - φορτωση τιμολογιου


    {


        public SqlConnection con;
        public IList<Monkey> Monkeys { get; private set; }
        public string f_cid="" ;
        public techn1()
        {
            InitializeComponent();
            Monkeys = new List<Monkey>();





        }
        protected override bool OnBackButtonPressed()
        {
            return false;
        }
        void  OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Monkey selectedItem = e.SelectedItem as Monkey;
            string c = selectedItem.idPEL;
            f_cid = c;
          /*  
          */
        }        
         async void diag_barcode(object sender, EventArgs e)
        {

            var action = await DisplayAlert("Να διαγραφεί?", "Εισαι σίγουρος?", "Ναι", "Οχι");
            if (action )
            {
                //  Navigate to first page
                MainPage.ExecuteSqlite("delete from PARALABES WHERE ID=" + f_cid );
                await DisplayAlert("διαγραφτηκε", "", "OK");
                show_list();
            }




        }
        async void delete_all(object sender, EventArgs e)
        {
            var action = await DisplayAlert("Να διαγραφoύν όλα τα τιμολόγια?", "Εισαι σίγουρος?", "Ναι", "Οχι");
            if (action)
            {
                //  Navigate to first page
                MainPage.ExecuteSqlite("delete from PARALABES ");
                await DisplayAlert("διαγραφτηκε", "", "OK");
                show_list();
            }
        }

        void SaveFile(string text)
        {
            //Get the SmbFile specifying the file name to be created.
            var file = new SmbFile("smb://" + Globals.cIP + "/eggtim2.txt");
            // fine var file = new SmbFile("smb://User:1@192.168.1.5/backpel/New2FileName.txt");
         
            if (file.Exists())
            {
                DisplayAlert("Θα διαγραφει το ηδη το αρχειο", "....", "OK");
                file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
            }
            else
            {
                //Create file.
                file.CreateNewFile();
            }
            

/*            try
            {
                //Create file.
                file.CreateNewFile();
            }
            catch
            {
                DisplayAlert("Θα διαγραφει το ηδη το αρχειο", "....", "OK");
                file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
            }
*/

            //Get writable stream.
            var writeStream = file.GetOutputStream();
          
            //Write bytes.
            writeStream.Write(Encoding.UTF8.GetBytes(text));

            //Dispose writable stream.
            writeStream.Dispose();
        }

        async void  CloseInvoice(object sender, EventArgs e)
        {
            await Navigation.PopAsync();



        }

        void show_list()
        {
            Monkeys = new List<Monkey>();
            BindingContext = null;

            string dbPath = Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.Personal),
              "adodemo.db3");
            bool exists = File.Exists(dbPath);
            if (!exists)
            {
                Console.WriteLine("Creating database");
                // Need to create the database before seeding it with some data
                Mono.Data.Sqlite.SqliteConnection.CreateFile(dbPath);

            }

            SqliteConnection connection = new SqliteConnection("Data Source=" + dbPath);
            // Open the database connection and create table with data
            connection.Open();

           
            var contents = connection.CreateCommand();
             contents.CommandText = "SELECT  * from PARALABES where ATIM ='"+cATIM.Text  +"' order by ID DESC ; "; // +BARCODE.Text +"'";
             // contents.CommandText = "SELECT  * from PARALABES ; "; // +BARCODE.Text +"'";
            var r = contents.ExecuteReader();
            Console.WriteLine("Reading data");
            while (r.Read())
            {
              

                Monkeys.Add(new Monkey
                {
                    Name = r["BARCODE"].ToString(),

                    Location = r["ATIM"].ToString(),
                    ImageUrl = "",
                    idPEL = r["id"].ToString ()
                });



            }
            



            connection.Close();
            
            BindingContext = this;
        }

        async void WriteFile(object sender, EventArgs e)
        {


            //  -----------------SQLSERVER  1.SYNDESH   ---------------------------------------

            string[] lines = Globals.cSQLSERVER.Split(';');
            string constring = @"Data Source=" + lines[0] + ";Initial Catalog=" + lines[1] + ";Uid=sa;Pwd=" + lines[2]; // ";Initial Catalog=MERCURY;Uid=sa;Pwd=12345678";

           // string constring = @"Data Source=" + Globals.cSQLSERVER + ";Initial Catalog=TECHNOPLASTIKI;Uid=sa;Pwd=12345678";
            con = new SqlConnection(constring);
            try
            {
                con.Open();
                // ***************  demo πως τρεχω εντολη στον sqlserver ********************************
                SqlCommand cmd = new SqlCommand("DELETE FROM PALETTIM ");
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
             //   cmd = new SqlCommand("insert into PALETESTEMP (ATIM,SSCC,HME) values ('','',GETDATE() )");
              //  cmd.Connection = con;
              //  cmd.ExecuteNonQuery();







            }
            catch (Exception ex)
            {
                await DisplayAlert("ΑΔΥΝΑΜΙΑ ΣΥΝΔΕΣΗΣ", ex.ToString(), "OK");
            }
            //  -----------------SQLSERVER ---------------------------------------













            // ΑΠΟΘΗΚΕΥΣΗ ΣΕ ΑΡΧΕΙΟ
            string dbPath = Path.Combine(
                     Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                     "adodemo.db3");
            SqliteConnection connection = new SqliteConnection("Data Source=" + dbPath);
            // Open the database connection and create table with data
            connection.Open();
            // query the database to prove data was inserted!
            var contents = connection.CreateCommand();
            contents.CommandText = "SELECT ATIM,BARCODE FROM PARALABES";
            var r = contents.ExecuteReader();
            // Console.WriteLine("Reading data");
            string cc = "";

            while (r.Read())
            {
                cc = cc + r["ATIM"].ToString() + ";";
                cc = cc + r["BARCODE"].ToString() + "\n";
                //cc = cc + r["POSO"].ToString() + ";";
                // cc = cc + r["TIMH"].ToString() + "\n";



                SqlCommand cmd = new SqlCommand("insert into PALETTIM(SCAN,ATIM,HME) values ('" + r["BARCODE"].ToString() + "','" + r["ATIM"].ToString() + "',GETDATE()  )");
                cmd.Connection = con;
                cmd.ExecuteNonQuery();




            }
            connection.Close();
           // SaveFile(cc);











            CrossToastPopUp.Current.ShowToastMessage("Αποθηκεύτηκε");

        }

        async void PaletaChanged(object sender, EventArgs e)
        {
            if (Globals.useBarcodes == "12")  // ενσωματωμενο σκανερ
            {
               
                    return;
                

               
            }
            if (Paleta.Text.Length == 0) return;

            BindingContext = null;
            Monkeys.Add(new Monkey
            {
                Name = Paleta.Text,

                Location = "***",
                ImageUrl = "---",
                idPEL = "///"
            });

            BindingContext = this;

             Paleta.Text = ""; // to ekana etsi gia na mporei na pairnei 2 fores tin idia paleta
            MainPage.ExecuteSqlite("INSERT INTO PARALABES (ATIM,BARCODE) VALUES ('" + cATIM.Text + "','" + Paleta.Text + "')");
           // Paleta.Text = ""; // to ekana etsi gia na mporei na pairnei 2 fores tin idia paleta
            show_list();

            Paleta.Focus();
        

        }
        async void barcfoc(object sender, EventArgs e)
        {
            string cc = "";
            if (Globals.useBarcodes == "12")  // ενσωματωμενο σκανερ
            {
                cc = Paleta.Text;

                return;
            }
           
            var scanPage = new ZXingScannerPage();
            // Navigate to our scanner page
            await Navigation.PushAsync(scanPage);

            scanPage.OnScanResult += (result) =>
            {

                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {


                    await Navigation.PopAsync();
                    // await DisplayAlert("Scanned Barcode", result.Text, "OK");
                    Thread.Sleep(500);
                    Paleta.Text = result.Text;

                    cc = Paleta.Text;




                });
            };













        }

        void KataxTimol(object sender, EventArgs e)
        {
           // show_list();
            Paleta.Focus();




        }


        private void PaletComplete(object sender, EventArgs e)
        {

            // διαβαζει απο ενσωματομενο σκανερ
            if (Globals.useBarcodes != "12")  // ενσωματωμενο σκανερ
            {
                return;
            }
            BindingContext = null;
            Monkeys.Add(new Monkey
            {
                Name = Paleta.Text,

                Location = "***",
                ImageUrl = "---",
                idPEL = "///"
            });

            BindingContext = this;

           // Paleta.Text = ""; // to ekana etsi gia na mporei na pairnei 2 fores tin idia paleta
            MainPage.ExecuteSqlite("INSERT INTO PARALABES (ATIM,BARCODE) VALUES ('" + cATIM.Text + "','" + Paleta.Text + "')");
            Paleta.Text = ""; // to ekana etsi gia na mporei na pairnei 2 fores tin idia paleta
            show_list();

            Paleta.Focus();

        }
    }
}