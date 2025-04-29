using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class SharedButton : MonoBehaviour
    {
        [SerializeField] ButtonType type;
        [SerializeField] Button button;
        [SerializeField] bool showModalWindow = true;

        void Start()
        {
            switch (type)
            {
                case ButtonType.Restart:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.restartModal.header,
                                    message = RGSKCore.Instance.UISettings.restartModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.restartModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.restartModal.declineButtonText,
                                    confirmAction = Restart,
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.restartModal.startSelection
                                });
                            }
                            else
                            {
                                Restart();
                            }
                        });

                        break;
                    }

                case ButtonType.BackToMenu:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.exitModal.header,
                                    message = RGSKCore.Instance.UISettings.exitModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.exitModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.exitModal.declineButtonText,
                                    confirmAction = () => SceneLoadManager.LoadMainScene(),
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.exitModal.startSelection
                                });
                            }
                            else
                            {
                                SceneLoadManager.LoadMainScene();
                            }
                        });

                        break;
                    }
                case ButtonType.BackToImmersionRace:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.exitModal.header,
                                    message = RGSKCore.Instance.UISettings.exitModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.exitModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.exitModal.declineButtonText,
                                    confirmAction = () => SceneLoadManager.LoadIntermissionRaceScene(),
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.exitModal.startSelection
                                });
                            }
                            else
                            {
                                SceneLoadManager.LoadIntermissionRaceScene();
                            }
                        });
                        break;
                    }

                case ButtonType.QuitApplication:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.quitModal.header,
                                    message = RGSKCore.Instance.UISettings.quitModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.quitModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.quitModal.declineButtonText,
                                    confirmAction = () => SceneLoadManager.QuitApplication(),
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.quitModal.startSelection
                                });
                            }
                            else
                            {
                                SceneLoadManager.QuitApplication();
                            }
                        });

                        break;
                    }

                case ButtonType.WatchReplay:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (RaceManager.Instance.Initialized &&
                                RaceManager.Instance.CurrentState != RaceState.PostRace)
                            {
                                return;
                            }

                            RecorderManager.Instance?.ReplayRecorder?.StartPlayback();
                        });
                        break;
                    }

                case ButtonType.DeleteSaveData:
                    {
                        button?.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = RGSKCore.Instance.UISettings.deleteSaveModal.header,
                            message = RGSKCore.Instance.UISettings.deleteSaveModal.message,
                            confirmButtonText = RGSKCore.Instance.UISettings.deleteSaveModal.confirmButtonText,
                            declineButtonText = RGSKCore.Instance.UISettings.deleteSaveModal.declineButtonText,
                            confirmAction = () => SaveManager.Instance?.DeleteSaveFile(),
                            declineAction = () => { },
                            startSelection = RGSKCore.Instance.UISettings.deleteSaveModal.startSelection
                        }));
                        break;
                    }
            }
        }

        void Restart()
        {
            PauseManager.Instance.Unpause();

            if (RaceManager.Instance.Initialized)
            {
                RaceManager.Instance.RestartRace();
                return;
            }

            SceneLoadManager.ReloadScene();
        }
    }
}