﻿using AdysTech.Influxer.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.Influxer
{
    public class GenericColumn : ITransform
    {

        public int ColumnIndex { get; set; }
        public string ColumnHeader { get; set; }
        public ColumnDataType Type { get; set; }
        public ColumnConfig Config { get; set; }

        public bool IsDefault
        {
            get; set;
        }

        public string DefaultValue
        {
            get; set;
        }


        List<ITransform> _transformations;
        List<GenericColumn> _generatedColumns;

        private void GetTransformations ()
        {
            _transformations = new List<ITransform> ();

            if (Config == null) return;
            foreach (var property in Config.GetType ().GetProperties ())
            {
                if (property.PropertyType.BaseType.IsGenericType)
                {
                    Type t = property.PropertyType.BaseType;
                    Type[] typeParameters = t.GetGenericArguments ();

                    foreach (Type tParam in typeParameters)
                    {
                        if (typeof (ITransform).IsAssignableFrom (tParam))
                        {
                            var list = property.GetValue (this.Config) as ICollection;
                            if (list != null && list.Count > 0)
                                foreach (ITransform transform in list)
                                {
                                    _transformations.Add (transform);
                                }
                        }
                    }

                }
                else if (typeof (ITransform).IsAssignableFrom (property.PropertyType))
                {
                    _transformations.Add (property as ITransform);
                }

            }
        }

        public bool HasTransformations
        {
            get
            {
                if (_transformations == null)
                    GetTransformations ();
                return (_transformations.Count > 0);
            }
        }


        public bool HasAutoGenColumns
        {
            get
            {
                if (_generatedColumns == null)
                    GetGeneratedColumns ();
                return (_generatedColumns.Count > 0);
            }
        }

        private void GetGeneratedColumns ()
        {
            _generatedColumns = new List<GenericColumn> ();
            if (this?.Config?.SplitConfig?.SubColumns == null) return;

            foreach (ColumnConfig c in this?.Config?.SplitConfig?.SubColumns)
            {
                if (!c.Skip)
                    _generatedColumns.Add (new GenericColumn () { ColumnHeader = c.InfluxName, ColumnIndex = -1, Type = c.DataType, Config = c });
            }
        }

        public bool CanTransform (string content)
        {
            if (!HasTransformations)
                return false;
            return _transformations.Any (t => t.CanTransform (content) == true);
        }

        public string Transform (string content)
        {
            if (HasTransformations)
            {
                var transformed = false;
                foreach (var t in _transformations.Where (t => !t.IsDefault))
                {
                    if (t.CanTransform (content))
                    {
                        content = t.Transform (content);
                        transformed = true;
                    }
                }
                if (!transformed && _transformations.Any (t => t.IsDefault))
                {
                    foreach (var t in _transformations.Where (t => t.IsDefault))
                    {
                        if (t.CanTransform (content))
                        {
                            content = t.Transform (content);
                        }
                    }
                }
            }
            return content;
        }


        public Dictionary<GenericColumn, string> SplitData (string content)
        {
            var result = new Dictionary<GenericColumn, string> ();
            if (Config.SplitConfig.CanSplit (content))
            {
                foreach (var split in Config.SplitConfig.Split (content))
                {
                    result.Add (_generatedColumns.FirstOrDefault (t => t.Config == split.Key), split.Value);
                }
            }
            else if (Config.SplitConfig.SubColumnsConfig.Any (t => t.IsDefault))
            {
                result.Add (_generatedColumns.FirstOrDefault (t => t.Config.IsDefault), content);
            }
            else
                throw new InvalidDataException ($"Can't split {content} using specified splitting rules, no default column configured");
            return result;
        }

    }
}
