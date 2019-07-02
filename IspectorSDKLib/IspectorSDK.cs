using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IspectorSDKLib
{
    public class Ispector
    {
        private string ispector_URL;


        public class SystemStatus
        {
            public double temperature { get; set; }
            public int eth_status { get; set; }
            public string eth_ip { get; set; }
            public int last_user_interact { get; set; }
            public string power { get; set; }
            public bool battery { get; set; }
            public int battery_life { get; set; }
            public int battery_charge { get; set; }
            public bool battery_in_charge { get; set; }
            public int remaining_time { get; set; }
            public int battery_voltage { get; set; }
            public int battery_current { get; set; }
            public int battery_temperature { get; set; }
            public int alarm { get; set; }
            public int httpcloud { get; set; }
            public int loracloud { get; set; }
        }

        public class Channel
        {
            public int id { get; set; }
            public bool HV_STATUS { get; set; }
            public double HV_VOLTAGE { get; set; }
            public string HV_MODE { get; set; }
            public bool COMPL_V { get; set; }
            public bool COMPL_I { get; set; }
            public double Vout { get; set; }
            public double Vref { get; set; }
            public double Iout { get; set; }
            public double IoutRAW { get; set; }
            public double Temp { get; set; }
            public double SetPoint { get; set; }
            public int ICR { get; set; }
            public int OCR { get; set; }
            public int runtime { get; set; }
            public int livetime { get; set; }
            public int sattime { get; set; }
            public int incnt { get; set; }
            public int outcnt { get; set; }
            public double live { get; set; }
            public double dead { get; set; }
            public int mca_running { get; set; }
            public int mca_status { get; set; }
        }

        public class CurrentStatus
        {
            public SystemStatus system_status { get; set; }
            public List<Channel> channels { get; set; }
        }

        public class IspectorStatus
        {
            public string command { get; set; }
            public string Result { get; set; }
            public int ErrorCode { get; set; }
            public string Reason { get; set; }
            public CurrentStatus current_status { get; set; }
        }


        public class WaveDump
        {
            public string command { get; set; }
            public string Result { get; set; }
            public int ErrorCode { get; set; }
            public string Reason { get; set; }
            public List<List<int>> data { get; set; }
        }


        public class Spectrum
        {
            public string command { get; set; }
            public string Result { get; set; }
            public int ErrorCode { get; set; }
            public string Reason { get; set; }
            public List<int> data { get; set; }
        }

        public enum TemperatureCompensation 
        {
            DISABLE_COMPENSATION,
            ENABLE_COMPENSATION
        } ;

        public enum RunMode
        {
            FREE = 0,
            TIME_MS = 1,
            COUNTS = 2
        };

        public enum BaselineLength
        {
            BASELINE_16 = 16,
            BASELINE_32 = 32,
            BASELINE_64 = 64,
            BASELINE_128 = 128,
            BASELINE_256 = 256,
            BASELINE_512 = 512,
            BASELINE_1024 = 1024
            
        };

        private bool HttpPostJson(string url, string json_data)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json_data);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string HttpGetJson(string url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }

                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public Ispector(string ip)
        {
            ispector_URL = "http://" + ip;
        }
        public bool set_hv_basic(bool hv_on, float hv_voltage)
        {
            string JSON_COMMAND = "{\"command\": \"SET_CHANNEL_CONFIG\", \"channel_config\": [{\"id\": 0, \"HV_STATUS\": " +
                       (hv_on ? "true" : "false") + ", \"HV_VOLTAGE\": " +
                       hv_voltage.ToString().Replace(",", ".") + "}], \"store_flash\": false}";
            return HttpPostJson(ispector_URL + "/set_config.cgi", JSON_COMMAND);

            
        }

        public bool set_hv_compensation(TemperatureCompensation mode, float temp_coeff)
        {
            string _mode = "digital";
            switch(mode)
            {
                case TemperatureCompensation.DISABLE_COMPENSATION:
                    _mode = "digital";
                    break;
                case TemperatureCompensation.ENABLE_COMPENSATION:
                    _mode = "temperature";
                    break;
            }
            string JSON_COMMAND = "{\"command\": \"SET_CHANNEL_CONFIG\", \"channel_config\": [{\"id\": 0, \"HV_MODE\": " +
                       _mode + ", \"TCoeff\": " +
                       temp_coeff.ToString().Replace(",", ".") + "}], \"store_flash\": false}";
            return HttpPostJson(ispector_URL + "/set_config.cgi", JSON_COMMAND);

             
        }

        public bool set_hv_cfg(int ramp, float maxI, float maxV, bool on_startup  )
        {
            string JSON_COMMAND = "{\"command\" : \"SET_CHANNEL_CONFIG\",\"channel_config\" :[{\"id\" : 0," +
                       "\"MaxV\" : " + maxV.ToString().Replace(",", ".") + "," +
                       "\"MaxI\" : " + maxI.ToString().Replace(",", ".") + "," +
                       "\"MaxT\" : 0, " +
                       "\"RAMP\" : " + ramp.ToString().Replace(",", ".") + "," +
                       "\"HV_PWRON\" : " + (on_startup ? "true" : "false") + "}],\"store_flash\" : false}";
            return HttpPostJson(ispector_URL + "/set_config.cgi", JSON_COMMAND);
        }

        public bool configureMCA(int trigger_threshold,
                     int trigger_inibit, int pre_int_time,
                     float int_time, int int_gain, float pileup_inib,
                     float pileup_penality, float baseline_inib,
                     BaselineLength baseline_len, RunMode target_run,
                     int target_value)
        {
            string JSON_COMMAND = "{\"command\" : \"SET_CHANNEL_CONFIG\",\"mca_config\" :[{\"id\" : 0," +
                       "\"trigger_thrs\" : " + trigger_threshold.ToString().Replace(",", ".") + "," +
                       "\"trigger_inib\" : " + trigger_inibit.ToString().Replace(",", ".") + "," +
                       "\"int_pre\" : " + pre_int_time.ToString().Replace(",", ".") + "," +
                       "\"int_val\" : " + int_time.ToString().Replace(",", ".") + "," +
                       "\"int_gain\" : " + int_gain.ToString().Replace(",", ".") + "," +
                       "\"pileup_inib\" : " + pileup_inib.ToString().Replace(",", ".") + "," +
                       "\"pileup_pen\" : " + pileup_penality.ToString().Replace(",", ".") + "," +
                       "\"baseline_inib\" : " + baseline_inib.ToString().Replace(",", ".") + "," +
                       "\"baseline_len\" : " + (int)baseline_len + "," +
                       "\"taget_run\" : " + (int)target_run + "," +
                       "\"taget_value\" : " + target_value.ToString().Replace(",", ".") + "," +
                       "\"reset_on_apply\" : true}],\"store_flash\" : false}}";
            return HttpPostJson(ispector_URL+ "/set_config.cgi", JSON_COMMAND);
        }


        public Channel getChannelStatus()
        {
            IspectorStatus IS;
            string json = HttpGetJson(ispector_URL + "/status.cgi");
            try { 
                IS = JsonConvert.DeserializeObject<IspectorStatus>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
            return IS.current_status.channels[0];
        }


        public SystemStatus getSystemStatus()
        {
            IspectorStatus IS;
            string json = HttpGetJson(ispector_URL + "/status.cgi");
            try
            {
                IS = JsonConvert.DeserializeObject<IspectorStatus>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
            return IS.current_status.system_status;
        }


        public List<List<int>> getWave()
        {
            WaveDump IS;
            string json = HttpGetJson(ispector_URL + "/wavedump.cgi");
            try
            {
                IS = JsonConvert.DeserializeObject<WaveDump>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
            return IS.data;
        }

        public List<int> getSpectrum()
        {
            Spectrum IS;
            string json = HttpGetJson(ispector_URL + "/spectrum.cgi");
            try
            {
                IS = JsonConvert.DeserializeObject<Spectrum>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
            return IS.data;
        }

        public void resetSpectrum()
        {
            HttpGetJson(ispector_URL + "/resetspectrum.cgi");
        }

        public void runSpectrum()
        {
            HttpGetJson(ispector_URL + "/mca_run.cgi");
        }

        public void stopSpectrum()
        {
            HttpGetJson(ispector_URL + "/mca_stop.cgi");
        }

        

    }
}
