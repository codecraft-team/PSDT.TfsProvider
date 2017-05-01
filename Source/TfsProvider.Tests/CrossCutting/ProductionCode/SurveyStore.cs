namespace PSDT.TfsProvider.Tests.CrossCutting.ProductionCode {
  internal class SurveyStore : ISurveyStore {
    public int Save(Survey survey) {
      return survey.Value;
    }
  }
}