using System.Threading;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;

namespace ThumbTest
{
	public partial class ThumbTestPage : ContentPage
	{
		public ThumbTestPage()
		{
			InitializeComponent();
		}

		private CancellationTokenSource _cancel;

		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			//var fpService = Resolve<IFingerprint>(); // or use dependency injection and inject IFingerprint

			try
			{
				//bool isAvailable = await Plugin.Fingerprint.CrossFingerprint.Current.IsAvailableAsync();
				FingerprintAvailability availability = await Plugin.Fingerprint.CrossFingerprint.Current.GetAvailabilityAsync();

				if (availability.Equals(FingerprintAvailability.Available))
				{
					AuthenticationRequestConfiguration authrequestConfig = new AuthenticationRequestConfiguration("reason");
					authrequestConfig.AllowAlternativeAuthentication = false;
					authrequestConfig.UseDialog = true;
					authrequestConfig.CancelTitle = "CancelTitle";
					authrequestConfig.FallbackTitle = "Fallback Title Text";

					 _cancel = new CancellationTokenSource();

					var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(authrequestConfig, _cancel.Token);
					if (result.Authenticated)
					{
						this.lblResult.Text = "User authorized.";
					}
					else
					{
						switch (result.Status)
						{
							case FingerprintAuthenticationResultStatus.FallbackRequested:
							case FingerprintAuthenticationResultStatus.TooManyAttempts:
		                        this.lblResult.Text = "Go to login page ...";
								await Navigation.PushAsync(new LoginPage(), animated: true);

								break;

						case FingerprintAuthenticationResultStatus.Canceled:
							case FingerprintAuthenticationResultStatus.Failed:
							case FingerprintAuthenticationResultStatus.Unknown:
							case FingerprintAuthenticationResultStatus.UnknownError:
		                        this.lblResult.Text = "Not allowed!";

								break;
						}

					}
				}
				else
				{
					this.lblResult.Text = availability.ToString();
				}
			}
			catch (System.Exception ex)
			{

			}
		}
	}
}
