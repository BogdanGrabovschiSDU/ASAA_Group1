using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class Order
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Order));
    public string Model { get; set; }

    public Order(string doc)
    {

        JObject json = new();
        try
        {
            json = JObject.Parse(doc);
        }
        catch (Exception ex)
        {

            log.Error("400 Bad Request: Could not Parse Json");
            throw new JsonException("400 Bad Request");
        }

        if (Validate(json))
        {

            //TODO order recived logic

        }
        else {
            throw new KeyNotFoundException();
        }
    }


    public bool Validate(JObject json)
    {

        if (json.TryGetValue("Model", out JToken? model))
        {

            if (Enum.TryParse(model.ToString(), out Models _))
            {

                return true;
            }
        }

        return false;

    }




}
