using Microsoft.ML;
using System;

namespace SysProgProjekat_TreciDeo
{
    public class ReviewService
    {
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;
        private readonly object _lock = new object();

        public ReviewService()
        {
            var mlContext = new MLContext();

            var data = mlContext.Data.LoadFromTextFile<SentimentData>(
                path: "Data/sentiment.csv",
                hasHeader: true,
                separatorChar: ';');

            var pipeline = mlContext.Transforms.Text.FeaturizeText(
                                outputColumnName: "Features",
                                inputColumnName: nameof(SentimentData.SentimentText))
                            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);

            _predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }

        public SentimentPrediction AnalyzeReview(string text)
        {
            var input = new SentimentData { SentimentText = text };
            lock (_lock)
            {
                return _predictionEngine.Predict(input);
            }
        }
    }

}