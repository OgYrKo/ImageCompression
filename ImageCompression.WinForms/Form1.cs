using System;
using System.Drawing;
using System.IO;
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
        private IAlgorithm CurrentAlgorithm { get; set; }
        private readonly string BMPFilter = "Изображения BMP|*.bmp|Все файлы|*.*";
        private readonly string BinFilter = "Bin Files (*.bin)|*.bin|All Files (*.*)|*.*";

        public Form1()
        {
            InitializeComponent();
            AlgorithmsComboBox.DataSource = Enum.GetValues(typeof(Algorithm));
            CurrentAlgorithm = new RunLengthEncoding(3);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Добавление обработчика события для контекстного меню PictureBox
            ContextMenu contextMenu = new ContextMenu();
            MenuItem copyMenuItem = new MenuItem("Копировать");
            MenuItem pasteMenuItem = new MenuItem("Вставить");
            copyMenuItem.Click += CopyMenuItem_Click;
            pasteMenuItem.Click += PasteMenuItem_Click;
            contextMenu.MenuItems.Add(copyMenuItem);
            contextMenu.MenuItems.Add(pasteMenuItem);
            UncompressedPicture.ContextMenu = contextMenu;

            // Добавление обработчика события для горячих клавиш Ctrl+C и Ctrl+V
            UncompressedPicture.KeyDown += UncompressedPicture_KeyDown;
        }
        

        #region <--- Work with file --->
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
        private byte[] OpenFile(string filter)
        {
            // Создаем диалоговое окно для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Устанавливаем фильтр для файлов с расширением .bmp
            openFileDialog.Filter = filter;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                return ReadBMPFile(openFileDialog.FileName);
            else
                return null;
        }
        private void OpenButton_Click(object sender, EventArgs e)
        {
            LoadNewImage(OpenFile(BMPFilter));
        }

        private void LoadNewImage(byte[] image)
        {
            ImageBytes = image;
            if (ImageBytes == null)
                MessageBox.Show("Ошибка открытия файла");
            else
            {
                DrawImage(ImageBytes, UncompressedPicture);
                PictureDispose(CompressedPicture);
            }
            ResultTextBox.Text = "";
            SaveCompressedFileButton.Enabled = false;
            DecompressButton.Enabled = false;
            SaveButton.Enabled = true;
            CompressionButton.Enabled = true;
        }
        private void OpenCompressedFile_Click(object sender, EventArgs e)
        {
            CompressedImageBytes = OpenFile(BinFilter);
            if (CompressedImageBytes == null)
                MessageBox.Show("Ошибка открытия файла");
            PictureDispose(CompressedPicture);
            PictureDispose(UncompressedPicture);
            CompressionButton.Enabled = false;
            SaveButton.Enabled = false;
            DecompressButton.Enabled = true;
            SaveCompressedFileButton.Enabled = true;

        }
        private void SaveFile(byte[] bytesToSave, string filter)
        {
            if (bytesToSave != null && bytesToSave.Length > 0)
            {
                // Создаем диалоговое окно для выбора местоположения и имени файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = filter;
                saveFileDialog.FilterIndex = 1; // Устанавливаем индекс фильтра по умолчанию

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Записываем байты в выбранный файл
                        File.WriteAllBytes(saveFileDialog.FileName, bytesToSave);
                        MessageBox.Show("Файл успешно сохранен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет данных для сохранения.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFile(ImageBytes, BMPFilter);
        }
        private void SaveCompressedFile_Click(object sender, EventArgs e)
        {
            SaveFile(CompressedImageBytes, BinFilter);
        }
        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Image != null)
            {
                Clipboard.SetImage(GetImageFromBytes(ImageBytes));
            }
        }
        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                LoadNewImage(GetBytesFromImage(Clipboard.GetImage()));
            }
        }
        #endregion <--- Work with file --->

        #region <--- Picture Drawing -->
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
        private static byte[] GetBytesFromImage(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp); // Можете выбрать другой формат, если нужно
                return stream.ToArray();
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
        private void PictureDispose(PictureBox pictureBox)
        {
            if (pictureBox.Image != null)
                pictureBox.Image.Dispose();
            pictureBox.Image = null;
        }
        #endregion <--- Picture Drawing -->

        #region <--- Compression --->
        private void CompressionButton_Click(object sender, EventArgs e)
        {
            if (ImageBytes == null)
            {
                MessageBox.Show("Не выбрано изображение для сжатия: ", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                CompressedImageBytes = CurrentAlgorithm.Compress(ImageBytes);
                stopwatch.Stop();
                SetCompressionResult(ImageBytes.LongLength, CompressedImageBytes.LongLength, stopwatch.ElapsedMilliseconds);
                DecompressButton.Enabled = true;
                SaveCompressedFileButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetCompressionResult(long uncompressedImageSize, long compressedImageSize, long compressionTime)
        {
            ResultTextBox.Text = GetCompressionResultString(uncompressedImageSize, compressedImageSize, compressionTime);
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
                byte[] decompressedImage = CurrentAlgorithm.Decompress(CompressedImageBytes);
                stopwatch.Stop();
                SetDecompressedResult(stopwatch.ElapsedMilliseconds);
                if (ImageBytes != null)
                {
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
                }
                else
                {
                    ImageBytes = decompressedImage;
                }
                DrawImage(decompressedImage, CompressedPicture);
                SaveButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetDecompressedResult(long decompressionTime)
        {
            ResultTextBox.Text += $"Время восстановления: {decompressionTime} мс\r\n";
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
        #endregion <--- Compression --->
        
        private void UncompressedPicture_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyMenuItem_Click(sender, e);
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                PasteMenuItem_Click(sender, e);
            }
        }
        private void AlgorithmsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Преобразуйте sender в ComboBox, чтобы получить выбранный элемент
            ComboBox comboBox = (ComboBox)sender;

            // Получите выбранный элемент перечисления
            Algorithm selectedAlgorithm = (Algorithm)comboBox.SelectedItem;

            switch (selectedAlgorithm)
            {
                case Algorithm.RLE:
                    CurrentAlgorithm = new RunLengthEncoding(3);
                    break;
                case Algorithm.LZW:
                    CurrentAlgorithm = new LempelZivWelch();
                    break;
                default:
                    MessageBox.Show("Алгоритм не реализован");
                    break;
            }
        }
    }
}
