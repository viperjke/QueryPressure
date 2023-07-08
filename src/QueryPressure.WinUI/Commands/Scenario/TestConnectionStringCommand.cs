using Microsoft.Extensions.Logging;
using QueryPressure.Core.Interfaces;
using QueryPressure.WinUI.Common.Commands;
using QueryPressure.WinUI.Dtos;
using QueryPressure.WinUI.Services.Core;
using QueryPressure.WinUI.Services.Language;
using System.Windows;

namespace QueryPressure.WinUI.Commands.Scenario;

public class TestConnectionStringCommand : CommandBase<TestConnectionStringDto>
{
  private readonly IProviderManager _providerManager;
  private readonly ILanguageService _languageService;

  public TestConnectionStringCommand(ILogger<TestConnectionStringCommand> logger, IProviderManager providerManager,
    ILanguageService languageService) : base(logger)
  {
    _providerManager = providerManager;
    _languageService = languageService;
  }

  protected override void ExecuteInternal(TestConnectionStringDto parameter)
  {
    _providerManager.GetProvider(parameter.Provider)
      .TestConnectionAsync(parameter.ConnectionString).ContinueWith(CheckTestResult);
  }

  private void CheckTestResult(Task<IServerInfo> task)
  {
    var strings = _languageService.GetStrings();
    try
    {

      var result = task.Result;

      if (result is null)
      {
        MessageBox.Show(
        string.Format(strings["labels.dialogs.failed-test-connectionString.message"], "NULL Result"),
        strings["labels.dialogs.failed-test-connectionString.title"],
        MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else
      {
        MessageBox.Show(
        string.Format(strings["labels.dialogs.succeed-test-connectionString.message"], result.ServerVersion),
        strings["labels.dialogs.succeed-test-connectionString.title"],
        MessageBoxButton.OK, MessageBoxImage.Information);
      }
    }
    catch (Exception ex)
    {
      var message = ex.InnerException?.Message ?? ex.Message;
      MessageBox.Show(
        string.Format(strings["labels.dialogs.failed-test-connectionString.message"], message),
        strings["labels.dialogs.failed-test-connectionString.title"],
        MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
