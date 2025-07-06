namespace Demo.ViewModel
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    using Demo.Model;
    using Nito.Mvvm;

    public class FileHashViewModel : ViewModelBase
    {
        private readonly IFileSystem _fileSystem;

        public FileHashViewModel(IDemoImageHash imageHash, IFileSystem fileSystem)
        {
            if (imageHash == null)
            {
                throw new ArgumentNullException(nameof(imageHash));
            }

            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            LoadCommand = new CapturingExceptionAsyncCommand(
                async () =>
                {
                    try
                    {
                        Busy = true;
                        var filename = FileName;
                        Image = await Task.Run(() => LoadImg(filename));
                        AverageHash = await Task.Run(() => imageHash.CalculateAverageHash(filename));
                        DifferenceHash = await Task.Run(() => imageHash.CalculateDifferenceHash(filename));
                        PerceptualHash = await Task.Run(() => imageHash.CalculatePerceptualHash(filename));
                        Loaded = true;
                    }
                    finally
                    {
                        Busy = false;
                    }
                },
                () => !Busy && !string.IsNullOrWhiteSpace(FileName));

            ClearCommand = new CapturingExceptionAsyncCommand(
                () =>
                {
                    Initialize();
                    return Task.CompletedTask;
                },
                () => !Busy);

            PropertyChanged += (sender, args) =>
            {
                LoadCommand.OnCanExecuteChanged();
                ClearCommand.OnCanExecuteChanged();
            };
        }

        public bool Loaded
        {
            get => Properties.Get(false);
            private set => Properties.Set(value);
        }

        public bool Busy
        {
            get => Properties.Get(false);
            set => Properties.Set(value);
        }

        public BitmapImage Image
        {
            get => Properties.Get(new BitmapImage());
            set => Properties.Set(value);
        }

        public byte[] AverageHash
        {
            get => Properties.Get<byte[]>([]);
            private set => Properties.Set(value);
        }

        public string AverageHashString => BitConverter.ToString(AverageHash);

        public byte[] DifferenceHash
        {
            get => Properties.Get<byte[]>([]);
            private set => Properties.Set(value);
        }

        public string DifferentialhashString => BitConverter.ToString(DifferenceHash);

        public byte[] PerceptualHash
        {
            get => Properties.Get<byte[]>([]);
            private set => Properties.Set(value);
        }

        public string PerceptualhashString => BitConverter.ToString(PerceptualHash);

        public string FileName
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public CapturingExceptionAsyncCommand LoadCommand { get; }

        public CapturingExceptionAsyncCommand ClearCommand { get; }

        private BitmapImage LoadImg(string file)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = _fileSystem.OpenRead(file);
            bitmapImage.EndInit();

            // https://stackoverflow.com/questions/26361020/error-must-create-dependencysource-on-same-thread-as-the-dependencyobject-even
            bitmapImage.Freeze();

            return bitmapImage;
        }

        private void Initialize()
        {
            Loaded = false;
            Image = new BitmapImage();
            AverageHash = BitConverter.GetBytes(0UL);
            DifferenceHash = BitConverter.GetBytes(0UL);
            PerceptualHash = BitConverter.GetBytes(0UL);
            FileName = string.Empty;
        }
    }
}
