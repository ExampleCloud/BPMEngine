﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    public sealed class ProcessVariablesContainer
    {
        private List<sVariableEntry> _variables;
        private int _stepIndex;

        private BusinessProcess _process = null;
        internal void SetProcess(BusinessProcess process) { _process = process; }

        public ProcessVariablesContainer()
        {
            _variables = new List<sVariableEntry>();
            _stepIndex = -1;
        }

        internal ProcessVariablesContainer(string elementID, ProcessState state,BusinessProcess process)
        {
            Log.Debug("Producing Process Variables Container for element[{0}]", new object[] { elementID });
            _stepIndex = state.Path.CurrentStepIndex(elementID);
            _variables = new List<sVariableEntry>();
            _process = process;
            foreach (string str in state[elementID])
            {
                Log.Debug("Adding variable {0} to Process Variables Container for element[{1}]", new object[] { str,elementID });
                _variables.Add(new sVariableEntry(str, _stepIndex, state[elementID, str]));
            }
        }

        public object this[string name]
        {
            get
            {
                object ret = null;
                bool found = false;
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (sve.Name == name)
                        {
                            found = true;
                            ret = sve.Value;
                            break;
                        }
                    }
                }
                if (!found && _process != null)
                    ret = _process[name];
                else if (!found && BusinessProcess.Current != null)
                    ret = BusinessProcess.Current[name];
                return ret;
            }
            set
            {
                lock (_variables)
                {
                    for (int x = 0; x < _variables.Count; x++)
                    {
                        if (_variables[x].Name == name)
                        {
                            _variables.RemoveAt(x);
                            break;
                        }
                    }
                    _variables.Add(new sVariableEntry(name, _stepIndex, value));
                }
            }
        }

        public string[] Keys
        {
            get
            {
                List<string> ret = new List<string>();
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (!ret.Contains(sve.Name))
                            ret.Add(sve.Name);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}
