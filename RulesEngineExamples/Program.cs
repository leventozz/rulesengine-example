using Newtonsoft.Json;
using RulesEngine.Models;
using RulesEngine.Extensions;

decimal transferAmount = 500;
dynamic input1 = new BankAccount();
dynamic input2 = transferAmount;

var inputs = new dynamic[]
    {
        input1,
        input2
    };


var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "TransferRules.json", SearchOption.AllDirectories);
if (files == null || files.Length == 0)
    throw new Exception("Rules not found.");

var fileData = File.ReadAllText(files[0]);
var workflow = JsonConvert.DeserializeObject<List<Workflow>>(fileData);

var bre = new RulesEngine.RulesEngine(workflow.ToArray(), null);

string message = string.Empty;

List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("Discount", inputs).Result;

resultList.OnSuccess((eventName) =>
{
    message = "Money transfer took place";
});

resultList.OnFail(() =>
{
    var failedRule = resultList.Find(r => !r.IsSuccess);
    if (failedRule != null)
    {
        message = failedRule.ExceptionMessage;
    }
    else
        message = "exception";
});

Console.WriteLine(message);

class BankAccount
{
    public string AccountNumber { get; set; } = "123456789";
    public int Balance { get; set; } = 1000;
}