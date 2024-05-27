using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CoordinateToArray
{
    public partial class Form1 : Form
    {
        private List<Point> points = new List<Point>();
        private Point? startPoint = null;
        private Point? endPoint = null;
        private bool drawing = false;
        private bool immagine = false;
        private Point previousMousePosition;

        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;
            this.MouseDown += Form1_MouseDown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && immagine)
            {
                int delta = e.Delta;
                float scaleFactor = delta > 0 ? 1.1f : 0.9f;
                pictureBox1.Width = (int)(pictureBox1.Width * scaleFactor);
                pictureBox1.Height = (int)(pictureBox1.Height * scaleFactor);
                pictureBox1.Refresh();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && immagine)
            {
                previousMousePosition = e.Location;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (immagine)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (!drawing)
                    {
                        startPoint = e.Location;
                        endPoint = null;
                        points.Add(startPoint.Value);
                        drawing = true;
                    }
                    else
                    {
                        endPoint = e.Location;
                        DrawPoint(endPoint.Value);
                        DrawLine();
                        GeneratePointsOnLine();
                        startPoint = endPoint;
                    }
                }
            }
            else
            {
                MessageBox.Show("Inserici prima l'immagine!");
            }
        }

        private void DrawPoint(Point point)
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.FillEllipse(Brushes.Red, point.X - 3, point.Y - 3, 6, 6);
            }
            pictureBox1.Refresh();
        }

        private void DrawLine()
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.DrawLine(Pens.Blue, startPoint.Value, endPoint.Value);
            }
            pictureBox1.Refresh();
        }

        private void GeneratePointsOnLine()
        {
            int dx = endPoint.Value.X - startPoint.Value.X;
            int dy = endPoint.Value.Y - startPoint.Value.Y;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xIncrement = (float)dx / steps;
            float yIncrement = (float)dy / steps;

            float x = startPoint.Value.X;
            float y = startPoint.Value.Y;

            for (int i = 1; i <= steps; i++)
            {
                points.Add(new Point((int)Math.Round(x), (int)Math.Round(y)));
                x += xIncrement;
                y += yIncrement;
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    immagine = true;
                    pictureBox1.Image = new Bitmap(openFileDialog.FileName);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Si è verificato un errore durante il caricamento dell'immagine: " + ex.Message);
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.Write("[");
                    for (int i = 0; i < points.Count; i++)
                    {
                        writer.Write($"[{points[i].X}, {points[i].Y}]");
                        if (i < points.Count - 1)
                        {
                            writer.Write(", ");
                        }
                    }
                    writer.Write("],");
                }
                MessageBox.Show("Coordinate salvate con successo!");
            }
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            immagine = false;
        }
    }
}
