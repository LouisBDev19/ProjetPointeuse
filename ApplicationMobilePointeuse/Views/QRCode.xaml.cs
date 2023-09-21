using QRCoder;

namespace ApplicationMobilePointeuse.Views;

public partial class QRCode : ContentPage
{
	public QRCode()
	{
		InitializeComponent();
    }

    private void GetQRCode(object sender, EventArgs e)
    {
        ImageSource qrCode = GenerateQRCode();
        if(!qrCode.IsEmpty)
        {
            MyQRCode.Source = qrCode;
            QRCodeButton.IsVisible = false;
            QRCodeText.Text = "QR Code généré avec succès ! Veuillez désormais le scanner";
        }
    }

    public static ImageSource GenerateQRCode()
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(DeviceInfo.Current.Platform.ToString(), QRCodeGenerator.ECCLevel.L);

        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);
        ImageSource qrImageSource = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));

        return qrImageSource;

    }

    public void ResetDisplay()
    {
        MyQRCode.Source = "qrcode";
        QRCodeButton.IsVisible = true;
        QRCodeText.Text = "Vous pouvez générer un QR Code en appuyant sur le bouton ci-dessous";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ResetDisplay();
    }
}