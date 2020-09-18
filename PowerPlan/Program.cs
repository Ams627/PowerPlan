using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace PowerPlan
{

    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Plans:");
                var plans = PlanHelper.GetPlans();

                var names = from plan in plans
                            let result = PlanHelper.ReadFriendlyName(plan)
                            let reportingName = result.error == 0 ? result.name : $"friendly name not available: error {result.error}"
                            select new { Guid = plan, Name = reportingName };


                names.ToList().ForEach(x => Console.WriteLine($"    {x.Guid} {x.Name}"));

                foreach (var plan in plans)
                {
                    Console.WriteLine($"plan: {plan}");
                    var settingsGroups = PlanHelper.GetSettingsGroups(plan);

                    var settingsGroupsDetails = from settingGroup in settingsGroups
                            let result = PlanHelper.ReadFriendlyName(plan, settingGroup)
                            let reportingName = result.error == 0 ? result.name : $"friendly name not available: error {result.error}"
                            select new { Guid = settingGroup, Name = reportingName };


                    foreach (var setting in settingsGroupsDetails)
                    {
                        Console.WriteLine($"    {setting.Guid} {setting.Name}");
                        PrintIndividualSettings(plan, setting.Guid);
                    }
                }

                Console.WriteLine();
                var currentPlan = PlanHelper.GetCurrentPlan();
                Console.WriteLine("Current Plan:");
                Console.WriteLine($"    {currentPlan} {PlanHelper.ReadFriendlyName(currentPlan)}");
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }

        }

        private static void PrintIndividualSettings(Guid plan, Guid subGroup)
        {
            var settings = PlanHelper.GetSettings(plan, subGroup);
            var settingsDetails = from setting in settings 
                                  let result = PlanHelper.ReadFriendlyName(plan, subGroup, setting)
                                  let reportingName = result.error == 0 ? result.name : $"friendly name not available: error {result.error}"
                                  select new { Guid = setting, Name = reportingName };

            foreach (var setting in settingsDetails)
            {
                if (!PlanHelper.IsHidden(subGroup, setting.Guid))
                {
                    Console.WriteLine($"        {setting.Guid} {setting.Name}");
                    var acValue = PlanHelper.GetAcValue(plan, subGroup, setting.Guid);
                    if (acValue is byte[] arr)
                    {
                        var printResult = string.Join(" ", arr.Select(x => $"{x:X2}"));
                        Console.WriteLine($"            {printResult}");
                    }
                    else if (acValue is UInt32 dword)
                    {
                        Console.WriteLine($"            {dword}");
                    }
                    else if (acValue is UInt64 qword)
                    {
                        Console.WriteLine($"            {qword}");
                    }
                    else if (acValue is string s)
                    {
                        Console.WriteLine($"            {s}");
                    }
                    else 
                    {
                        Console.WriteLine($"            {acValue}");
                    }
                }
            }
        }
    }
}
