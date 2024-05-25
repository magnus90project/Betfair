using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Math;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.MachineLearning;
using FootyStatsAPI;

namespace FootyStatsAnalyser
{
    public class FootyStatsAnalyzer
    {
        private FootyStatsApiClient _apiClient;

        public FootyStatsAnalyzer(FootyStatsApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        /*
        public static async Task<List<PredictionResult>> GetPremiumPredictionsAsync(FootyStatsApiClient apiClient)
        {
            var data = await apiClient.GetPremiumPredictionsAsync();

            var matrix = data.Select(d => new double[]
            {
                    d.HomeTeamShots, d.AwayTeamShots, d.HomeTeamShotsOnTarget, d.AwayTeamShotsOnTarget,
                    d.HomeTeamGoals, d.AwayTeamGoals, d.HomeTeamCorners, d.AwayTeamCorners,
                    d.HomeTeamYellowCards, d.AwayTeamYellowCards, d.HomeTeamRedCards, d.AwayTeamRedCards
            }).ToArray();

            var labels = data.Select(d => d.Prediction == PredictionResultType.HomeWin ? 1 : 0).ToArray();

            var inputs = new double[data.Count][];

            for (int i = 0; i < data.Count; i++)
            {
                inputs[i] = new double[matrix[i].Length + 1];
                matrix[i].CopyTo(inputs[i], 0);
                inputs[i][matrix[i].Length] = 1; // Konstantterm
            }

            var learner = new LogisticRegressionAlgorithm();
            var model = learner.Learn(inputs, labels);

            var result = new List<PredictionResult>();

            for (int i = 0; i < data.Count; i++)
            {
                var prediction = model.Decide(inputs[i]) == 1 ? PredictionResultType.HomeWin : PredictionResultType.AwayWin;
                result.Add(new PredictionResult
                {
                    MatchId = data[i].MatchId,
                    HomeTeam = data[i].HomeTeam,
                    AwayTeam = data[i].AwayTeam,
                    HomeTeamShots = data[i].HomeTeamShots,
                    AwayTeamShots = data[i].AwayTeamShots,
                    HomeTeamShotsOnTarget = data[i].HomeTeamShotsOnTarget,
                    AwayTeamShotsOnTarget = data[i].AwayTeamShotsOnTarget,
                    HomeTeamGoals = data[i].HomeTeamGoals,
                    AwayTeamGoals = data[i].AwayTeamGoals,
                    HomeTeamCorners = data[i].HomeTeamCorners,
                    AwayTeamCorners = data[i].AwayTeamCorners,
                    HomeTeamYellowCards = data[i].HomeTeamYellowCards,
                    AwayTeamYellowCards = data[i].AwayTeamYellowCards,
                    HomeTeamRedCards = data[i].HomeTeamRedCards,
                    AwayTeamRedCards = data[i].AwayTeamRedCards,
                    Prediction = prediction
                });

            }

            return result;
        }
        */
    }

  

}
