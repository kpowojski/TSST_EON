using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CarrierAndSlotsConverter
    {

        public static string[] convertFromBitRate(string bitRate)
        {
            Random rnd = new Random();
            string[] carrierAndSlots = new string[2];

            int slots;
            double carrier;
            switch (bitRate)
            {
                case "10":
                    slots= Constants.SLOTS_FOR_BITRATE_10;
                    break;
                case "40":
                    slots = Constants.SLOTS_FOR_BITRATE_40;
                    break;
                case "100":
                    slots = Constants.SLOTS_FOR_BITRATE_100;
                    break;
                default:
                    slots = Constants.SLOTS_FOR_BITRATE_10;
                    break;
            }
            //double max = Constants.MAX_FREQUENCY;
            //double min = Constants.MIN_FREQUENCY;
            carrier = rnd.NextDouble() * 1136 * Constants.DISTANCE_BETWEEN_FREQUENCIES + Constants.CENRTAL_FREQUENCY;

            decimal dec1 = (decimal)carrier;
            dec1 = Math.Round(dec1, 2);
            Console.WriteLine(dec1);
            carrierAndSlots[0] = Convert.ToString(dec1);


            carrierAndSlots[1] = Convert.ToString(slots);
            return carrierAndSlots; 
        }

    }
}
