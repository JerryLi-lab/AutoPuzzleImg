using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		

		private void OnOpenClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				selectNames = openFileDialog.FileNames;
			}
		}

		private void OnOutPutClick(object sender, EventArgs e)
		{
			FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBox1.Text = openFileDialog.SelectedPath;
			}
		}

		private string[] selectNames = null;


		private void OnPuzzleClick(object sender, EventArgs e)
		{
			if (selectNames != null && textBox1.Text.Length != 0)
			{
				foreach (string fileName in selectNames)
				{
					Bitmap rowImage0 = (Bitmap)Image.FromFile(fileName);
					Bitmap rowImage = rowImage0.Clone(new Rectangle(0, 0, rowImage0.Width, rowImage0.Height), PixelFormat.Format32bppArgb);
					Bitmap tempImg = rowImage.Clone(new Rectangle(0, 0, rowImage.Width, rowImage.Height), PixelFormat.Format32bppArgb);
					List<KeyValuePair<int, int>> images = new List<KeyValuePair<int, int>>();
					int row = tempImg.Height / 128;
					int col = tempImg.Width / 96;
					for (int i = 0; i < row; ++i)
					{
						for (int j = 0; j < col; ++j)
						{
							images.Add(new KeyValuePair<int, int>(i, j));
						}
					}


					for (int i = row - 1; i >= 0; --i)
					{
						for (int j = col - 1; j >= 0; --j)
						{
							float minTotal = float.MaxValue;

							KeyValuePair<int, int> matchPart = new KeyValuePair<int, int>();

							foreach (KeyValuePair<int, int> kv in images)
							{
								float rowTotal = 0;
								for (int k = 0; k < 128; ++k)
								{
									int rowIndex1 = kv.Value * 96 + 95;
									int colIndex2 = kv.Key * 128 + k;
									Color color1 = rowImage.GetPixel(rowIndex1, colIndex2); // 原来的
									int rowIndex3 = j * 96 + 96;
									int colIndex4 = i * 128 + k;
									Color color2 = tempImg.GetPixel(rowIndex3, colIndex4);
									int rowIndex5 = j * 96 + 96;
									int colIndex6 = Math.Max(0, i * 128 + k - 1);
									Color color3 = tempImg.GetPixel(rowIndex5, colIndex6);
									int rowIndex7 = j * 96 + 96;
									int colIndex8 = i * 128 + k + 1;
									Color color4 = tempImg.GetPixel(rowIndex7, colIndex8);

									float avgR = (Math.Abs(color2.R - color1.R) + Math.Abs(color3.R - color2.R) + Math.Abs(color4.R - color3.R)) / 3;
									float avgG = (Math.Abs(color2.G - color1.G) + Math.Abs(color2.G - color2.G) + Math.Abs(color2.G - color3.G)) / 3;
									float avgB = (Math.Abs(color2.B - color1.B) + Math.Abs(color2.B - color2.B) + Math.Abs(color2.B - color3.B)) / 3;

									float total = (avgR + avgG + avgB) / 3;

									{
										rowTotal += total;

									}
								}

								rowTotal /= 127;
								float colTotal = 0;
								for (int u = 0; u < 95; ++u)
								{
									Color color1 = rowImage.GetPixel(kv.Value * 96 + u, kv.Key * 128 + 127); // 原来的
									Color color2 = tempImg.GetPixel(j * 96 + u, i * 128 + 128);

									Color color3 = tempImg.GetPixel(Math.Max(0, (j * 96 + u - 1)), i * 128 + 128);
									Color color4 = tempImg.GetPixel(j * 96 + u + 1, i * 128 + 128);


									float avgR = (Math.Abs(color2.R - color1.R) + Math.Abs(color3.R - color2.R) + Math.Abs(color4.R - color3.R)) / 3;
									float avgG = (Math.Abs(color2.G - color1.G) + Math.Abs(color2.G - color2.G) + Math.Abs(color2.G - color3.G)) / 3;
									float avgB = (Math.Abs(color2.B - color1.B) + Math.Abs(color2.B - color2.B) + Math.Abs(color2.B - color3.B)) / 3;

									float total = (avgR + avgG + avgB) / 3;

									{
										colTotal += total;

									}
								}
								colTotal /= 95;

								if (Math.Abs(colTotal + rowTotal) < minTotal)
								{
									minTotal = Math.Abs(colTotal + rowTotal);
									matchPart = kv;
								}
							}

							images.Remove(matchPart);

							for (int g = 0; g < 128; ++g)
							{
								for (int r = 0; r < 96; ++r)
								{
									Color tempColor = rowImage.GetPixel(matchPart.Value * 96 + r, matchPart.Key * 128 + g);
									Color tempColor2 = Color.FromArgb(tempColor.A, tempColor.R, tempColor.G, tempColor.B);
									tempImg.SetPixel(j * 96 + r, i * 128 + g, tempColor2);

								}
							}
						}
					}
					string[] names = fileName.Split('.');
					string[] names2 = names[0].Split('\\');
					tempImg.Save(textBox1.Text + names2[names2.Length - 1] + "-Automatic puzzle." + names[1]);
					pictureBox1.Image = tempImg;
					pictureBox1.Width = tempImg.Width;
					pictureBox1.Height = tempImg.Height;
					vScrollBar1.Maximum = tempImg.Height;
					//pictureBox1.Scale(new SizeF(0.3f, 0.3f));
				}
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		public void MyScrollEventHandler(object sender, ScrollEventArgs e)
		{
			
			pictureBox1.Location = new Point(pictureBox1.Location.X, 10 + (-e.NewValue));
			 
		}
	}
}
