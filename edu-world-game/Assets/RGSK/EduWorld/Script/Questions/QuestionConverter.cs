
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RGSK
{
    public class QuestionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BaseQuestion);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObj = JObject.Load(reader);
            var type = jsonObj["type"]?.ToString();

            BaseQuestion question = type switch
            {
                "fill_blank" => new FillBlankQuestion(),
                "matching" => new MatchingQuestion(),
                "multiple_choice" => new MultipleChoiceQuestion(),
                "ordering" => new OrderingQuestion(),
                "true_false" => new TrueFalseQuestion(),
                _ => throw new NotSupportedException($"Unknown question type: {type}")
            };

            serializer.Populate(jsonObj.CreateReader(), question);
            return question;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Optional: Nếu bạn cần serialize lại
            throw new NotImplementedException();
        }
    }
}
