using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageCompression.Algorithms;
using ImageCompression.Interfaces;
using System.Diagnostics;
namespace ImageCompression.WinForms
{
    public partial class Form1 : Form
    {
        private byte[] ImageBytes { get; set; }
        private byte[] CompressedImageBytes { get; set; }
        private IAlgorithm Algorithm { get; set; }

        public Form1()
        {
            InitializeComponent();
            Algorithm = new RunLengthEncoding(1);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            // Создаем диалоговое окно для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Устанавливаем фильтр для файлов с расширением .bmp
            openFileDialog.Filter = "Изображения BMP|*.bmp|Все файлы|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageBytes = ReadBMPFile(openFileDialog.FileName);
                DrawImage(ImageBytes, UncompressedPicture);
                PictureDispose(CompressedPicture);
                ResultTextBox.Text = "";
            }
        }

        private byte[] ReadBMPFile(string filePath)
        {
            try
            {
                // Проверяем, существует ли файл
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Файл не найден", filePath);
                }

                // Читаем все байты из файла
                byte[] fileBytes = File.ReadAllBytes(filePath);

                return fileBytes;
            }
            catch (Exception ex)
            {
                // Обработка ошибок чтения файла

                MessageBox.Show("Ошибка чтения файла BMP: " + ex.Message);
                return null;
            }
        }

        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private static Image GetImageFromBytes(byte[] bmpBytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(bmpBytes))
                {
                    // Создаем изображение из массива байтов
                    Image image = Image.FromStream(stream);

                    return image;
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок создания изображения
                MessageBox.Show("Ошибка создания изображения из массива байтов: " + ex.Message);
                return null;
            }
        }

        private void DrawImage(byte[] bytes, PictureBox pictureBox) 
            => DrawImage(GetImageFromBytes(bytes), pictureBox);

        private void DrawImage(Image image, PictureBox pictureBox)
        {

            try
            {
                // Освобождаем ресурсы
                PictureDispose(pictureBox);
                // Создаем новое изображение с максимальными размерами PictureBox
                Image scaledImage = ScaleImage(image, pictureBox.Width, pictureBox.Height);

                // Рассчитываем положение изображения для центрирования
                int x = (pictureBox.Width - scaledImage.Width) / 2;
                int y = (pictureBox.Height - scaledImage.Height) / 2;

                // Создаем новый Bitmap для отображения изображения по центру
                Bitmap centeredImage = new Bitmap(pictureBox.Width, pictureBox.Height);
                using (Graphics g = Graphics.FromImage(centeredImage))
                {
                    g.DrawImage(scaledImage, x, y); // Рисуем масштабированное изображение по центру

                }

                // Отображаем центрированное изображение в PictureBox
                pictureBox.Image = centeredImage;
                scaledImage.Dispose();
            }
            catch (Exception ex)
            {
                // Обработка ошибок загрузки изображения
                MessageBox.Show("Ошибка при открытии изображения: " + ex.Message);
            }
        }

        private void PictureDispose(PictureBox pictureBox)
        {
            if (pictureBox.Image != null)
                pictureBox.Image.Dispose();
            pictureBox.Image = null;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {

        }

        private void CompressionButton_Click(object sender, EventArgs e)
        {
            if (ImageBytes == null)
            {
                MessageBox.Show("Не выбрано изображение для сжатия: ", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CompressedImageBytes = Algorithm.Compress(ImageBytes);
            stopwatch.Stop();
            SetCompressionResult(ImageBytes.LongLength, CompressedImageBytes.LongLength, stopwatch.ElapsedMilliseconds);
        }

        private void SetCompressionResult(long uncompressedImageSize, long compressedImageSize, long compressionTime)
        {
            ResultTextBox.Text = GetCompressionResultString(uncompressedImageSize, compressedImageSize, compressionTime);
        }

        private void SetDecompressedResult(long decompressionTime)
        {
            ResultTextBox.Text += $"Время восстановления: {decompressionTime} мс\n\r";
        }

        private string GetCompressionResultString(long uncompressedImageSize, long compressedImageSize, long compressionTime)
        {
            string result = $"Размер исходного изображения: {uncompressedImageSize} байт\r\n";
            result += $"Размер сжатого изображения: {compressedImageSize} байт\r\n";
            result += $"Коэффициент сжатия: {(double)uncompressedImageSize / compressedImageSize}\r\n";
            result += $"Время сжатия: {compressionTime} мс\r\n";
            return result;
        }

        private void DecompressButton_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                byte[] decompressedImage = Algorithm.Decompress(CompressedImageBytes);
                stopwatch.Stop();
                SetDecompressedResult(stopwatch.ElapsedMilliseconds);
                bool areEqual = AreArraysEqual(ImageBytes, decompressedImage);
                if (!areEqual)
                {
                    int index = FirstNotEqualIndex(ImageBytes, decompressedImage);
                    if (index != -1)
                    {
                        (byte[], byte[]) arrs = GetNeighbours(ImageBytes, decompressedImage, 51, index);
                        byte[] arr1 = arrs.Item1, arr2 = arrs.Item2;
                    }
                    MessageBox.Show($"Исходные данные и распакованные данные {(areEqual ? "совпадают" : "не совпадают")}");
                }
                DrawImage(decompressedImage, CompressedPicture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool AreArraysEqual(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }

            return true;
        }

        private int FirstNotEqualIndex(byte[] arr1, byte[] arr2)
        {
            int i = -1;
            while (++i < Math.Min(arr1.Length, arr2.Length))
            {
                if (arr1[i] != arr2[i]) return i;
            }
            return -1;
        }

        private (byte[], byte[]) GetNeighbours(byte[] arr1, byte[] arr2, int count, int index)
        {
            int startIndex = index - count / 2;
            (byte[], byte[]) arrs = (new byte[count], new byte[count]);
            for (int i = 0; i < count; i++, startIndex++)
            {
                arrs.Item1[i] = arr1[startIndex];
                arrs.Item2[i] = arr2[startIndex];
            }
            return arrs;
        }
    }
}
