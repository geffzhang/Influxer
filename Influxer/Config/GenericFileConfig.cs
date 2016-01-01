﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.Influxer.Config
{
    public class GenericFileConfig : PerfmonFileConfig
    {
        [CommandLineArgAttribute ("-table", Usage = "-table <table name>", Description = "Measurement name in InfluxDB", DefaultValue = "InfluxerData")]
        [ConfigurationProperty ("TableName", DefaultValue = "InfluxerData")]
        public string TableName
        {
            get { return (string) this["TableName"]; }
            set { this["TableName"] = value; }
        }

        [CommandLineArgAttribute ("-utcoffset", Usage = "-utcoffset <No of Minutes>", Description = "Offset in minutes to UTC, each line in input will be adjusted to arrive time in UTC")]
        [ConfigurationProperty ("UtcOffset", DefaultValue = 0)]
        public int UtcOffset
        {
            get { return (int) this["UtcOffset"]; }
            set { this["UtcOffset"] = value; }
        }


        [CommandLineArgAttribute ("-validate", Usage = "-validate <No of Rows>", Description = "Validates n rows for consistent column data types")]
        [ConfigurationProperty ("ValidateRows", DefaultValue = 10)]
        public int ValidateRows
        {
            get { return (int) this["ValidateRows"]; }
            set { this["ValidateRows"] = value; }
        }

        [CommandLineArgAttribute ("-header", Usage = "-header <Row No>", Description = "Indicates which row to use to get column headers")]
        [ConfigurationProperty ("HeaderRow", DefaultValue = 1)]
        public int HeaderRow
        {
            get { return (int) this["HeaderRow"]; }
            set { this["HeaderRow"] = value; }
        }

        [CommandLineArgAttribute ("-skip", Usage = "-skip <Row No>", Description = "Indicates how may roaws should be skipped after header row to get data rows")]
        [ConfigurationProperty ("SkipRows", DefaultValue = 0)]
        public int SkipRows
        {
            get { return (int) this["SkipRows"]; }
            set { this["SkipRows"] = value; }
        }

    }
}