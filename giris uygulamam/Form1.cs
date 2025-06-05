using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Text; // SQL Server ile bağlantı için gerekli kütüphane

namespace giris_uygulamam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Çıkmak istediğinize emin misiniz?", "Çıkış İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
            if (onay == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Resources\\Database1.mdf;Integrated Security=True");
            baglanti.Open(); // Veritabanı bağlantısını aç
            SqlCommand komut = new SqlCommand("SELECT * FROM Kullanici where KullaniciAdi=@KullaniciAdi AND parola=@parola", baglanti);
            komut.Parameters.AddWithValue("@KullaniciAdi", textBox1.Text);
            komut.Parameters.AddWithValue("@parola", textBox2.Text);
            SqlDataReader dr = komut.ExecuteReader(); // Veritabanından kullanıcı bilgilerini oku
            
            if (dr.Read())
            {
                baglanti.Close(); // Veritabanı bağlantısını kapat
                icerik frm = new icerik(); // Yeni formu oluştur
                frm.Show(); // Yeni formu göster
                this.Visible = false; // Mevcut formu gizle
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya parola yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); // Hata mesajı göster
            }

        }
    }
}
