using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL Server ile bağlantı için gerekli kütüphane
using System.IO;
using System.Diagnostics.Eventing.Reader;
namespace giris_uygulamam
{
    public partial class ekle : Form
    {
        public ekle()
        {
            InitializeComponent();
        }

        string resimYolu; //Resim yolu için değişken
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Resim Seç"; // OpenFileDialog başlığı
            openFileDialog1.Filter = "Jpeg Dosyası(*.jpg|*.jpg|gif Dosyası(*.gif)|*.gif|png Dosyası(*.png)|*.png"; // Resim dosyası filtreleri

            if (openFileDialog1.ShowDialog() == DialogResult.OK) // Kullanıcı bir dosya seçerse
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // Resmi PictureBox'a sığacak şekilde esnet
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName); // Seçilen resmi pictureBox'a atama
                resimYolu = openFileDialog1.FileName.ToString(); // Resim yolunu değişkene atama
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)  // Eğer resim seçilmemişse
            {
                MessageBox.Show("Lütfen bir resim seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else 
            {
                FileStream fs = new FileStream(resimYolu, FileMode.Open, FileAccess.Read); // Resim dosyasını açma
                BinaryReader br = new BinaryReader(fs); // BinaryReader ile dosyayı okuma
                byte[] resim = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                SqlConnection baglanti = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Resources\\Database1.mdf;Integrated Security=True");
                baglanti.Open(); // Veritabanı bağlantısını açma
                SqlCommand komut = new SqlCommand("select * from icerik where tc_no=@tc_no", baglanti); // SQL sorgusu
                komut.Parameters.AddWithValue("@tc_no", textBox3.Text); // Parametre ekleme
                SqlDataReader dr = komut.ExecuteReader(); // Sorguyu çalıştırma

                if (dr.Read()) // Eğer kayıt varsa
                {
               
                    MessageBox.Show("Bu TC Kimlik Numarası zaten kayıtlı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    dr.Close();
                    SqlCommand kmt = new SqlCommand("insert into icerik (ad,soyad,tc_no,dogum_yili, dogum_yeri,resim) values (@ad, @soyad, @tc_no, @dogum_yili, @dogum_yeri, @image)", baglanti);
                    kmt.Parameters.Add("@ad", textBox1.Text); // name parameters
                    kmt.Parameters.Add("@soyad", textBox2.Text); // surname parameters
                    kmt.Parameters.Add("@tc_no", textBox3.Text); // TC number parameters
                    kmt.Parameters.Add("@dogum_yili", textBox4.Text); // Birth year parameters
                    kmt.Parameters.Add("@dogum_yeri", textBox5.Text); // Birth place parameters
                    kmt.Parameters.Add("@image", SqlDbType.Image, resim.Length).Value = resim; // Image parameters
                    kmt.ExecuteNonQuery();
                    MessageBox.Show("Kayıt başarılı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    baglanti.Close(); // Veritabanı bağlantısını kapatma

                    icerik frm = new icerik(); // İçerik formunu açma
                    frm.Show(); // İçerik formunu gösterme
                    this.Visible = false; // Mevcut formu gizleme

                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Kayıt işleminden çıkış yapıyorsunuz", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (onay == DialogResult.Yes) // Eğer kullanıcı "Evet" seçerse
            {
                icerik frm = new icerik();
                frm.Show(); // İçerik formunu göster
                this.Visible = false; // Mevcut formu gizle  
            }
    }
}
