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
using System.IO;

namespace giris_uygulamam
{
    public partial class ekle : Form
    {
        public ekle()
        {
            InitializeComponent();
        }

        string resimYolu; // Resim yolunu saklamak için değişken

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Resim Seç";
            openFileDialog1.Filter = "Jpeg Dosyası (*.jpg)|*.jpg|Gif Dosyası (*.gif)|*.gif|Png Dosyası (*.png)|*.png";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                resimYolu = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Lütfen bir resim seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                FileStream fs = new FileStream(resimYolu, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] resim = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();

                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Resources\\Database1.mdf;Integrated Security=True";
                using (SqlConnection baglanti = new SqlConnection(connectionString))
                {
                    baglanti.Open();

                    // TC No'nun daha önce var olup olmadığını kontrol et
                    SqlCommand komut = new SqlCommand("SELECT * FROM icerik WHERE tc_no=@tc_no", baglanti);
                    komut.Parameters.AddWithValue("@tc_no", textBox3.Text);
                    SqlDataReader dr = komut.ExecuteReader();

                    if (dr.Read())
                    {
                        MessageBox.Show("Bu TC Kimlik Numarası zaten kayıtlı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dr.Close();
                        return;
                    }
                    else
                    {
                        dr.Close();

                        // Yeni kayıt ekleme işlemi
                        SqlCommand kmt = new SqlCommand("INSERT INTO icerik (ad, soyad, tc_no, dogum_yili, dogum_yeri, resim) VALUES (@ad, @soyad, @tc_no, @dogum_yili, @dogum_yeri, @image)", baglanti);

                        kmt.Parameters.Add("@ad", SqlDbType.NVarChar).Value = textBox1.Text;
                        kmt.Parameters.Add("@soyad", SqlDbType.NVarChar).Value = textBox2.Text;
                        kmt.Parameters.Add("@tc_no", SqlDbType.NVarChar).Value = textBox3.Text;
                        kmt.Parameters.Add("@dogum_yili", SqlDbType.NVarChar).Value = textBox4.Text;
                        kmt.Parameters.Add("@dogum_yeri", SqlDbType.NVarChar).Value = textBox5.Text;

                        SqlParameter imageParam = new SqlParameter("@image", SqlDbType.Image, resim.Length);
                        imageParam.Value = resim;
                        kmt.Parameters.Add(imageParam);

                        kmt.ExecuteNonQuery();
                        MessageBox.Show("Kayıt başarılı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        icerik frm = new icerik();
                        frm.Show();
                        this.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Kayıt işleminden çıkış yapıyorsunuz. Devam etmek istiyor musunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (onay == DialogResult.Yes)
            {
                icerik frm = new icerik();
                frm.Show();
                this.Visible = false;
            }
        }
    }
}