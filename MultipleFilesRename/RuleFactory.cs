using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using RuleBaseInterface;

namespace MultipleFilesRename
{
    public class RuleFactory
    {
        static Dictionary<string, IRule> _prototypes = new Dictionary<string, IRule>();

        public ObservableCollection<IRule> Produce()
        {
            var result = new ObservableCollection<IRule>();
            foreach (var i in _prototypes.Values)
                result.Add(i);
            return result;
        }
        public static void Register(IRule prototype)
        {
            _prototypes.Add(prototype.Name, prototype);
        }

        private static RuleFactory? _instance = null;
        public static RuleFactory Instance()
        {
            if (_instance == null)
            {
                _instance = new RuleFactory();
            }

            return _instance;
        }
        public IRule Parse(string data)
        {
            var tokens = data.Split(new string[] { " " },
                StringSplitOptions.None);
            var keyword = tokens[0];
            IRule? result = null;

            if (_prototypes.ContainsKey(keyword))
            {
                IRule prototype = _prototypes[keyword];
                result = prototype.Parse(data);
            }

            return result;
        }
    }
    public partial class MainWindow
    {
        void LoadDllFile()
        {
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var folderInfo = new DirectoryInfo(exeFolder);
            var dllFiles = folderInfo.GetFiles("*.dll");

            foreach (var file in dllFiles)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass && typeof(IRule).IsAssignableFrom(type))
                    {
                        IRule rule = (IRule)Activator.CreateInstance(type)!;
                        RuleFactory.Register(rule);
                    }
                }
            }

            _activeRules.Clear();
            _activeRules = RuleFactory.Instance().Produce();
        }
    }
}
