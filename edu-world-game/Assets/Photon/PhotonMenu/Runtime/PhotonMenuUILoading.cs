namespace Fusion.Menu
{
  /// <summary>
  /// The loading screen.
  /// </summary>
  public partial class PhotonMenuUILoading : PhotonMenuUIScreen
  {
    public override void Show()
    {
      base.Show();
      LoadingScreenManager.instance.ShowLoading(LoadingScreenType.Loading);
    }

    public override void Hide()
    {
      base.Hide();
      LoadingScreenManager.instance.HideLoading();
    }

    /// <summary>
    /// Is called when the <see cref="_disconnectButton"/> is pressed using SendMessage() from the UI object.
    /// </summary>
    protected virtual async void OnDisconnectPressed()
    {
      await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
    }
  }
}
