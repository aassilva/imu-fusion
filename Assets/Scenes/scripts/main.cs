using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.ComponentModel;


public class main : MonoBehaviour
{

    static AHRS.MadgwickAHRS AHRS = new AHRS.MadgwickAHRS(1f / 256f, 0.1f);
    //static AHRS.MahonyAHRS AHRS = new AHRS.MahonyAHRS(1f / 256f, 5f);

    static string path = "E:\\aassilva\\test_fusion\\imu.csv";
    static string pathFileRecovery = "E:\\aassilva\\test_fusion\\points_2.csv";

    // Start is called before the first frame update
    void Start() { 
        Vector3 point = new Vector3(0, 0, 0);
        List<Sensor> sensors = new List<Sensor>();
        sensors = ReaderIMU(path);

        //Inica a classe aqui.
        foreach (Sensor sensor in sensors) {

            //AHRS.Update(deg2rad(sensor.gyroscopeX), deg2rad(sensor.gyroscopeY), deg2rad(sensor.gyroscopeZ),
              //  sensor.accelerometerX, sensor.accelerometerY, sensor.accelerometerZ);

            AHRS.Update(deg2rad(sensor.gyroscopeX), deg2rad(sensor.gyroscopeY), deg2rad(sensor.gyroscopeZ),
                sensor.accelerometerX, sensor.accelerometerY, sensor.accelerometerZ, sensor.magnetometerX,
                    sensor.magnetometerY, sensor.magnetometerZ);

            float[] quaternionReturn = AHRS.GetQuaternios();

            Quaternion quaternion = new Quaternion(quaternionReturn[0], quaternionReturn[1], quaternionReturn[2], quaternionReturn[3]);

            Vector3 tmp = new Vector3(0, 0, 0);
            tmp = quaternion.eulerAngles;  //eulerAngles;

            point = point + (tmp + (Vector3.forward * 0.5f));

            WriterLineCSV(point);

            //Thread.Sleep(100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void WriterLineCSV(Vector3 point) {
        using (StreamWriter w = File.AppendText(pathFileRecovery)) // < app persistent datapath 
        {
            var first = point.x;
            var second = point.y;
            var third = point.z;
            var line = string.Format("{0},{1},{2}", first, second, third);
            w.WriteLine(line);
            w.Flush();
        }
    }

    float deg2rad(float degress) {

        return (float)((Math.PI / 180) * degress);
    }

    List<Sensor> ReaderIMU(string path) {

        using (var reader = new StreamReader(path)) {

            List<Sensor> sensors = new List<Sensor>();

            while (!reader.EndOfStream) {

                var line = reader.ReadLine();
                line = line.Replace("\"", string.Empty);
                var values = line.Split(',');

                Sensor sensor = new Sensor {

                    gyroscopeX = float.Parse(values[0]),
                    gyroscopeY = float.Parse(values[1]),
                    gyroscopeZ = float.Parse(values[2]),
                    accelerometerX = float.Parse(values[3]),
                    accelerometerY = float.Parse(values[4]),
                    accelerometerZ = float.Parse(values[5]),
                    magnetometerX = float.Parse(values[6]),
                    magnetometerY = float.Parse(values[7]),
                    magnetometerZ = float.Parse(values[8]),
                };

                sensors.Add(sensor);
            }

            return sensors;
        }
    }
}

class Sensor {

    public float gyroscopeX;
    public float gyroscopeY;
    public float gyroscopeZ;
    public float accelerometerX;
    public float accelerometerY;
    public float accelerometerZ;
    public float magnetometerX;
    public float magnetometerY;
    public float magnetometerZ;
}