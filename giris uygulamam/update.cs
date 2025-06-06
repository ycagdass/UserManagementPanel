using denemeVT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // SQL Server ile bağlantı için gerekli kütüphane
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace giris_uygulamam
{
    public partial class update : Form
    {
        public update()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Kayıt güncelleme işleminden çıkış yapıyorsunuz. Devam etmek istiyor musunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (onay == DialogResult.Yes)
            {
                icerik frm = new icerik();
                frm.Show();
                this.Visible = false;
            }
        }

        string resimYolu; // Resim yolunu saklamak için değişken
        bool kontrol = false; // Resim seçilip seçilmediğini kontrol etmek için değişken

        private void button1_Click(object sender, EventArgs e)
        {
            // OpenFileDialog'i ayarla
            openFileDialog1.Title = "Resim Seç";
            openFileDialog1.Filter =
                "Jpeg Dosyası (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +  // jpg ve jpeg
                "Gif Dosyası (*.gif)|*.gif|" +                  // gif
                "Png Dosyası (*.png)|*.png|" +                  // png
                "Bmp Dosyası (*.bmp)|*.bmp|" +                  // bmp
                "Tüm Resim Dosyaları (*.jpg;*.jpeg;*.gif;*.png;*.bmp)|*.jpg;*.jpeg;*.gif;*.png;*.bmp"; // Tüm resimler

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Resmi PictureBox'a yükle
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);

                // Resim yolunu sakla
                resimYolu = openFileDialog1.FileName;

                // Kontrol değişkenini true yap
                kontrol = true;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection cmd2 = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Resources\\Database1.mdf;Integrated Security=True");
            cmd2.Open();

            if (kontrol == false)
            {
                // Resim seçilmemişse
                SqlCommand upt = new SqlCommand("UPDATE icerik SET ad=@ad, soyad=@soyad, tc_no=@tc_no, dogum_yili=@dogum_yili, dogum_yeri=@dogum_yeri WHERE ID=@kimlik", cmd2);
                upt.Parameters.AddWithValue("@ad", textBox1.Text);
                upt.Parameters.AddWithValue("@soyad", textBox2.Text);
                upt.Parameters.AddWithValue("@tc_no", textBox3.Text);
                upt.Parameters.AddWithValue("@dogum_yili", textBox4.Text);
                upt.Parameters.AddWithValue("@dogum_yeri", textBox5.Text);
                upt.Parameters.AddWithValue("@kimlik", Program.Duzenlenecek_ID);
                upt.ExecuteNonQuery(); // Sorguyu çalıştır

                cmd2.Close(); // Bağlantıyı kapat

                MessageBox.Show("Resimsiz düzenleme başarılı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                icerik frm = new icerik();
                frm.Show(); // Yeni formu aç
                this.Close(); // Mevcut formu kapat
            }
            else
            {
                // PictureBox'daki resmi byte[] haline çevir
                byte[] img;
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    img = ms.ToArray();
                }

                // Resim seçilmişse
                SqlCommand uptWithImage = new SqlCommand(
                    "UPDATE icerik SET ad=@ad, soyad=@soyad, tc_no=@tc_no, dogum_yili=@dogum_yili, dogum_yeri=@dogum_yeri, resim=@resim WHERE ID=@kimlik",
                    cmd2);

                uptWithImage.Parameters.AddWithValue("@ad", textBox1.Text);
                uptWithImage.Parameters.AddWithValue("@soyad", textBox2.Text);
                uptWithImage.Parameters.AddWithValue("@tc_no", textBox3.Text);
                uptWithImage.Parameters.AddWithValue("@dogum_yili", textBox4.Text);
                uptWithImage.Parameters.AddWithValue("@dogum_yeri", textBox5.Text);
                uptWithImage.Parameters.Add("@resim", SqlDbType.Image).Value = img;
                uptWithImage.Parameters.AddWithValue("@kimlik", Program.Duzenlenecek_ID);

                uptWithImage.ExecuteNonQuery(); // Sorguyu çalıştır

                cmd2.Close(); // Bağlantıyı kapat

                MessageBox.Show("Resimli düzenleme başarılı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                icerik frm = new icerik();
                frm.Show(); // Yeni formu aç
                this.Close(); // Mevcut formu kapat
            }
        }
    }
}
