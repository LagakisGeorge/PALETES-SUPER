using System; // χτιστιμο παλετας
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
using Plugin.SimpleAudioPlayer;
using System.Data;

namespace test4sql // χτιστιμο παλετας
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class View1 : ContentPage
    {
        public IList<Monkey> Monkeys { get; private set; }
        public string f_cid = "";
        public SqlConnection con;
        private ISimpleAudioPlayer _simpleAudioPlayer;


        public View1() // χτιστιμο παλετας
        {
            InitializeComponent();
            Monkeys = new List<Monkey>();
            _simpleAudioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Stream beepStream = GetType().Assembly.GetManifestResourceStream("test4sql.beep.mp3");
            bool isSuccess = _simpleAudioPlayer.Load(beepStream);
            Show_list();


        }
        //protected override bool OnBackButtonPressed()
        //{
        //    return false;
        //}
        void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Monkey selectedItem = e.SelectedItem as Monkey;
            string c = selectedItem.idPEL;
            f_cid = c;
            /*  
            */
        }
        async void Diag_barcode(object sender, EventArgs e)
        {

            var action = await DisplayAlert("Να διαγραφεί?", "Εισαι σίγουρος?", "Ναι", "Οχι");
            if (action)
            {
                //  Navigate to first page
                MainPage.ExecuteSqlite("delete from PARALABESXT WHERE ID=" + f_cid);
                await DisplayAlert("διαγραφτηκε", "", "OK");
                Show_list();
            }




        }

        
        async void delete_all(object sender, EventArgs e)
        {
            var action = await DisplayAlert("Να διαγραφoύν όλα τα σκαναρισματα?", "Εισαι σίγουρος?", "Ναι", "Οχι");
            if (action)
            {
                //  Navigate to first page
                MainPage.ExecuteSqlite("delete from PARALABESXT ");
                await DisplayAlert("διαγραφτηκε", "", "OK");
                Show_list();
            }
        }
        
        async void SaveFile(string text)
        {

            // DESKTOP-MPGU8SB\SQL17
            string[] lines = Globals.cSQLSERVER.Split(';');
            string constring = @"Data Source=" + lines[0] + ";Initial Catalog=" + lines[1] + ";Uid=sa;Pwd=" + lines[2]; // ";Initial Catalog=MERCURY;Uid=sa;Pwd=12345678";

//            string constring = @"Data Source=" + Globals.cSQLSERVER + ";Initial Catalog=TECHNOPLASTIKI;Uid=sa;Pwd=12345678";
            // ok fine string constring = @"Data Source=DESKTOP-MPGU8SB\SQL17,51403;Initial Catalog=MERCURY;Uid=sa;Pwd=12345678";
            // ok works fine string constring = @"Data Source=192.168.1.10,51403;Initial Catalog=MERCURY;Uid=sa;Pwd=12345678";

            con = new SqlConnection(constring);

            try
            {
                con.Open();

                // await DisplayAlert("Συνδεθηκε", "οκ", "OK");

                // ***************  demo πως τρεχω εντολη στον sqlserver ********************************
                SqlCommand cmd = new SqlCommand("insert into PALETES(PALET) values (1)");
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                await DisplayAlert("ΑΔΥΝΑΜΙΑ ΣΥΝΔΕΣΗΣ", ex.ToString(), "OK");
            }

            return;

            try
            {


                //Get the SmbFile specifying the file name to be created.
                var file = new SmbFile("smb://" + Globals.cIP + "/eggtim2.txt");
                // fine var file = new SmbFile("smb://User:1@192.168.1.5/backpel/New2FileName.txt");

                if (file.Exists())
                {
                    DisplayAlert("Θα διαγραφει το παλιό αρχειο", "....", "OK");
                    file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
                }
                // else
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
            catch
            {
                DisplayAlert("Δεν γράφτηκε το αρχειο", "....", "OK");
                // file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
            }





        }

        async void CloseInvoice(object sender, EventArgs e)
        {
            await Navigation.PopAsync();



        }

        void Show_list()
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
            //  mono to sygkekrimeno where ATIM ='" + cATIM.Text + "'
            contents.CommandText = "SELECT  * from PARALABESXT  order by ID DESC ; "; // +BARCODE.Text +"'";
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
                    idPEL = r["id"].ToString()
                });



            }




            connection.Close();

            BindingContext = this;
        }

        async void WriteFile(object sender, EventArgs e)
        {


            // DIABAZO sqlite database KAI TA SOZO :
            // se sqlserver
            // KAI SE KOINO FAKELO SAN ARXEIO  EGGTIM2.TXT


            //  -----------------SQLSERVER  1.SYNDESH   ---------------------------------------

          

            string[] lines = Globals.cSQLSERVER.Split(';');
            string constring = @"Data Source=" + lines[0] + ";Initial Catalog=" + lines[1] + ";Uid=sa;Pwd=" + lines[2]; // ";Initial Catalog=MERCURY;Uid=sa;Pwd=12345678";

//            string constring = @"Data Source=" + Globals.cSQLSERVER + ";Initial Catalog=TECHNOPLASTIKI;Uid=sa;Pwd=12345678";
            con = new SqlConnection(constring);
            try
            {
                con.Open();                
            }
            catch (Exception ex)
            {
                await DisplayAlert("ΑΔΥΝΑΜΙΑ ΣΥΝΔΕΣΗΣ", ex.ToString(), "OK");
            }
            //  -----------------SQLSERVER ---------------------------------------


            string cc22 = cATIM.Text;
                if (cc22.Length <18)
            {
                await DisplayAlert("Λαθος SSCC", "", "OK");
                return;
            }


            DataSet ds = new DataSet();
            SqlCommand cmd2 = new SqlCommand("select top 1 ID FROM PALETES WHERE CONVERT(INT,PALET)=" + cATIM.Text.Substring(12, 7) + "", con);
            var adapter = new SqlDataAdapter(cmd2);
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count == 0)
            {
                await DisplayAlert("δεν υπαρχει  η παλετα "+ cATIM.Text.Substring(12, 7), "", "OK");
                return;
            }
            int mID = (int)ds.Tables[0].Rows[0][0]; // 201712552030 0217

            SqlCommand cmd = new SqlCommand("delete from PALETES WHERE CONVERT(INT,PALET)=" + cATIM.Text.Substring(12, 7) +  "");
            cmd.Connection = con;
            cmd.ExecuteNonQuery();




            /*
            // '=====================  EGGTIM2.TXT  ======================================================'
            try
            {
                //Get the SmbFile specifying the file name to be created.
                var file = new SmbFile("smb://" + Globals.cIP + "/eggtim2.txt");
            // fine var file = new SmbFile("smb://User:1@192.168.1.5/backpel/New2FileName.txt");
            if (file.Exists())
            {
                DisplayAlert("Θα διαγραφει το παλιό αρχειο", "....", "OK");
                file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
            }
           // else
            {
                //Create file.
                file.CreateNewFile();
            }
                       //try
                       // {
                       //     //Create file.
                       //     file.CreateNewFile();
                       // }
                       // catch
                       // {
                       //     DisplayAlert("Θα διαγραφει το ηδη το αρχειο", "....", "OK");
                       //     file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
                       // }


               //Get writable stream.
               var writeStream = file.GetOutputStream();

               //Write bytes.
               writeStream.Write(Encoding.UTF8.GetBytes(text));

               //Dispose writable stream.
               writeStream.Dispose();


            }
            catch
            {
                DisplayAlert("Δεν γράφτηκε το αρχειο", "....", "OK");
               // file.Delete();  // https://csharpdoc.hotexamples.com/class/SharpCifs.Smb/SmbFile#
            }
            // '=====================  EGGTIM2.TXT  ======================================================'
            */

            // ***************  demo πως τρεχω εντολη στον sqlserver ********************************
            //  SqlCommand cmd = new SqlCommand("insert into PALETES(PALET,KOD,PARTIDA,POSO) values (1)");
            //  cmd.Connection = con;
            //  cmd.ExecuteNonQuery();





            string dbPath = Path.Combine(
                     Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                     "adodemo.db3");
            SqliteConnection connection = new SqliteConnection("Data Source=" + dbPath);
            // Open the database connection and create table with data
            connection.Open();
            // query the database to prove data was inserted!
            var contents = connection.CreateCommand();
            contents.CommandText = "SELECT ATIM,BARCODE FROM PARALABESXT";
            var r = contents.ExecuteReader();
            // Console.WriteLine("Reading data");
            String cPal, cPart, cPos, cKod;

            string cc = "";


            try
            {


                while (r.Read())
                {
                    cc = cc + r["ATIM"].ToString() + ";";
                    cc = cc + r["BARCODE"].ToString() + "\n";
                    //cc = cc + r["POSO"].ToString() + ";";
                    // cc = cc + r["TIMH"].ToString() + "\n";  11  12->  21,5


                   

                    cPal = r["ATIM"].ToString();
                    cPal = cPal.Substring(12, 7);


                    cKod = r["BARCODE"].ToString();
                    cKod = "00" + cKod.Substring(11, 4);

                    cPos = r["BARCODE"].ToString();
                    cPos = cPos.Substring(19, 5);

                    cPart = r["BARCODE"].ToString();
                    cPart = cPart.Substring(27, 7);


                   


                    cmd = new SqlCommand("insert into PALETES(PALET,[KOD],[PARTIDA],[POSO]) values ('" + cPal + "','" + cKod + "','" + cPart + "'," + cPos + ")");
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    //}



                }
                connection.Close();



                CrossToastPopUp.Current.ShowToastMessage("Αποθηκεύτηκε");
                //  SaveFile(cc);

                MainPage.ExecuteSqlite("DELETE FROM  PARALABESXT");


            }
            catch (Exception ex)
            {
                await DisplayAlert("ΑΔΥΝΑΜΙΑ ΣΥΝΔΕΣΗΣ", ex.ToString(), "OK");
            }

        }

        async void PaletaChanged(object sender, EventArgs e)
        {
            if (Globals.useBarcodes == "12")  // ενσωματωμενο σκανερ
            {
                return;
            }
            if (Paleta.Text.Length < 34) return;

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
            MainPage.ExecuteSqlite("INSERT INTO PARALABESXT (ATIM,BARCODE) VALUES ('" + cATIM.Text + "','" + Paleta.Text + "')");
            Show_list();
            Paleta.Focus();

        }



        async void barcfoc(object sender, EventArgs e)
        {

            string cc = "";
            if (Globals.useBarcodes == "12")  // ενσωματωμενο σκανερ
            {
                

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


        async void paletfoc()
        {
            if (Globals.useBarcodes == "12")  // ενσωματωμενο σκανερ
            {
                

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
                    cATIM .Text = result.Text;
                   
                });
            };
        }



        void ViewPalet(object sender, EventArgs e)
        {
            Show_list();
            paletfoc();

          //  Paleta.Focus();




        }

        private void PALCOMPLETED(object sender, EventArgs e)
        {
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
            MainPage.ExecuteSqlite("INSERT INTO PARALABESXT (ATIM,BARCODE) VALUES ('" + cATIM.Text + "','" + Paleta.Text + "')");
            Show_list();
            Paleta.Text = "";
            Paleta.Focus();
        }

        private void arxcompleted(object sender, EventArgs e)
        {
            Paleta.Focus();
        }

        private void NewSSCC(object sender, EventArgs e)
        {
            cATIM.Text = "";
            Paleta.Text = "";
        }
    }
}