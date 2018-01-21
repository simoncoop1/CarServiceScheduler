using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public static class Constants{

    public static readonly String xmlFname = "CarScheduler.xml";

    public static readonly double daysinyear = 365.25;

}

public class Users
{
    public List<User> userAr = new List<User>();
    static private Users usersInstance;

    public static Users Instance(){
        if(usersInstance == null){
            usersInstance = new Users();
        }
        return usersInstance;
    }

    public void SerializeComplex(){
        XmlSerializer x = new XmlSerializer(typeof(Users));
        MemoryStream m = new MemoryStream();
        x.Serialize(m, Users.Instance());

        //write to memory first so if xml serialisation fails, xml file is not broke
        using (FileStream file = new FileStream(Constants.xmlFname, FileMode.Create, FileAccess.Write))
            m.WriteTo(file);

        //Textwriter t = ne StreamWriter(Constants.xmlFname);   
    }

    public void DeSerializeComplex(){
        XmlSerializer ser = new XmlSerializer(typeof(Users));
        Users usersDeSer;

        try{
            using (XmlReader reader = XmlReader.Create(Constants.xmlFname))
            {
                usersDeSer = (Users)ser.Deserialize(reader);
            }

            usersInstance = usersDeSer;
        }
        catch(System.IO.FileNotFoundException e)
        {

        }
    }
}

public class User{
    public String firstname;
    public String secondname;
    public String username;
    public String password;
    String email;
    String telephone;

    public List<Car> cars = new List<Car>();
    
}

public class Session{
    private static Session session;

    public User user;

    public Car car;

    public static Session Instance(){
        if(session == null)
            session = new Session();
        
        return session;
    }
}

public class Car{

    public Manufacturer manufacturer;


    public Model model;

    public string numberplate;

    public DateTime registrationdate;

    public List<Mileage> mileagerecording = new List<Mileage>();

    [XmlElement(typeof(OilChange))]
    [XmlElement(typeof(OilFilterChange))]
    [XmlElement(typeof(DustFilter))]
    [XmlElement(typeof(PollenFilter))]
    [XmlElement(typeof(FuelFilterChange))]
    [XmlElement(typeof(ToothedBelt))]
    public List<ServiceTask> servicetasks = new List<ServiceTask>();

    public Mileage MostRecentMileage(){

        return mileagerecording.Aggregate((i1,i2) => i1.date > i2.date ? i1 : i2);

    }

}

public class Manufacturer{



    public ManufacturerNames name;

    public List<Model> models = new List<Model>();
}

public class Model{
    public ModelNames name;

    public Double enginesize;

    public Fuel fuel;

    public Transmission transmission;

    public Doors doors;

    public Colour colour;

    public override string ToString(){
        return "Model: " + name.ToString() + "\n" +
            "Engine Size: " + enginesize.ToString() + "\n" +
            "Fuel: " + fuel.ToString() + "\n" +
            "Transmission: " + fuel.ToString() + "\n" +
            "Doors: " + doors.ToString() + "\n" +
            "Colour: " + colour.ToString();
    }

    public string ToStringShort(){
        return name.ToString() + " " + enginesize.ToString() + " " + fuel.ToString();
    }


}

public static class ManufacturerLibrary{

    public static List<Manufacturer> manufacturers;

    static ManufacturerLibrary(){
        manufacturers = new List<Manufacturer>();
        Manufacturer vw = new Manufacturer();
        vw.name = ManufacturerNames.Volkswagen;
        manufacturers.Add(vw);
        Model golf = new Model();
        vw.models.Add(golf);
        golf.name = ModelNames.Golf;
        golf.enginesize = 1.9;
        golf.fuel = Fuel.Diesel;
        golf.transmission = Transmission.Manual;
        golf.doors = Doors.doors5;
        golf.colour = Colour.Silver;
        

        Manufacturer skoda = new Manufacturer();
        skoda.name = ManufacturerNames.Skoda;
        manufacturers.Add(skoda);
        Model octavia = new Model();
        skoda.models.Add(octavia);
        octavia.colour = Colour.Silver;
        octavia.doors = Doors.doors5;
        octavia.enginesize = 1.9;
        octavia.fuel = Fuel.Diesel;
        octavia.name = ModelNames.Octavia;
        octavia.transmission = Transmission.Manual;
        Model citigo = new Model();
        skoda.models.Add(citigo);
        citigo.colour = Colour.Red;
        citigo.doors = Doors.doors5;
        citigo.enginesize = 1.0;
        citigo.fuel = Fuel.Petrol;
        citigo.name = ModelNames.CitiGo;
        citigo.transmission = Transmission.Manual;
        
    }

}

public enum Fuel { Diesel, Petrol};

public enum Transmission { Manual, Automatic};

public enum Doors { doors5, Doors3};
public enum Colour { Silver, Red, LightBlue};

public enum ManufacturerNames { Volkswagen, Skoda, Ford};

public enum ModelNames { Golf, Octavia, CitiGo,Fiesta};

public class CLI{

    public static void WriteLineLabelColonTerminated(String s){

        //is it multiline?

        String[] lines = s.Split('\n');
        if(lines.Length > 1){
            foreach(String line in lines){
                WriteLineLabelColonTerminated(line);
            }

            return;
        }

        String r = s.Substring(s.IndexOf(":")+1);
        String fw = s.Substring(0,s.IndexOf(":")+1);

        ConsoleColor cc = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(fw);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(r);

        //change console colour to whatever it was begining of the function
        Console.ForegroundColor = cc;

    }

        public static void WriteLineErrorMessage(String s){


        ConsoleColor cc = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(s);

        //change console colour to whatever it was begining of the function
        Console.ForegroundColor = cc;

    }

}

public enum CLIMenuEnd { Success, Fail,Other};

public class Mileage{

    public DateTime date;

    public int mileage;

    public static DateTime GetEstimatedDate(int mileage,List<Mileage> ml){

        DateTime meanx = new DateTime((long)(ml.Average(k => k.date.Ticks)));
        int meany = (int)ml.Average(k => k.mileage);

        //work out gradient m
        long num1 = ml.Sum(k => (k.date.Ticks-meanx.Ticks)*(k.mileage - meany));
        double num2 = ml.Sum(k => Math.Pow(k.date.Ticks-meanx.Ticks,2));
        double m = num1/num2;

        //work out y intercept
        double c = meany - (m*meanx.Ticks);

        //(y-c)/m
        return new DateTime((long)((mileage-c)/m));

    }

    public static int GetEstimatedMileage(DateTime date, List<Mileage> ml){

        DateTime meanx = new DateTime((long)(ml.Average(k => k.date.Ticks)));
        int meany = (int)ml.Average(k => k.mileage);

        //work out gradient m
        long num1 = ml.Sum(k => (k.date.Ticks-meanx.Ticks)*(k.mileage - meany));
        double num2 = ml.Sum(k => Math.Pow(k.date.Ticks-meanx.Ticks,2));
        double m = num1/num2;

        //work out y intercept
        double c = meany - (m*meanx.Ticks);

        //(y-c)/m
        return (int)(date.Ticks*m+c);
        
        
    }



}

public enum Repeat { Single, Repeat};

//[XmlRoot(Namespace = Constants.servicetasknamespace)]
//[XmlInclude(typeof(OilChange))]
//[XmlInclude(typeof(OilFilterChange))]
//[XmlInclude(typeof(DustFilter))]
//[XmlInclude(typeof(PollenFilter))]
//[XmlInclude(typeof(FuelFilterChange))]
//[XmlInclude(typeof(ToothedBelt))]
public abstract class ServiceTask{

    public string name;

    public string description;

    public List<ServiceTaskRecord>  servictaskrecord = new List<ServiceTaskRecord>();

    public Repeat repeat;

    public ServiceMileageInterval mileageInterval;

    public ServiceTimeInterval timeinterval;

    internal ServiceTaskRecord GetMostRecent(){

        return servictaskrecord.Aggregate((i1,i2) => i1.date > i2.date ? i1 : i2);

    }

    //for a given mileagerecord say when service task is due
    //return null if the service task is only base on mileage, or if the period is once
    public DateTime? GetWhenDue(Mileage m)
    {

        //single use service task
        if(repeat == Repeat.Single && this.servictaskrecord.Count > 1)
            return null;

        ServiceTaskRecord r;

        try{
            r = GetMostRecent();
        }
        catch(InvalidOperationException ie){
            return null;
        }
        

        //the service task has a mileage and time interval
        //if((mileageInterval != ServiceMileageInterval.na) && 
        //    (timeinterval != ServiceTimeInterval.na)){


        //the service task has a mileage interval only

        //the service task has time interval only

        //the service task has no mileage interval or time interval

       //the last recorded mileage is greater than the service task plus its mileage
       if(m.mileage>= (r.mileage+(int)mileageInterval)){

                //Console.WriteLine(r.mileage+(int)mileageInterval);
                //Console.WriteLine(m.mileage);
                return m.date;
       }

        //the service task does not have a time interval
        if(timeinterval == ServiceTimeInterval.na){
            return null;
        }


        int i = (int)timeinterval;

        TimeSpan ts = new TimeSpan((int)(i*Constants.daysinyear),0,0,0);

        return r.date + ts;

    }

    public DateTime GetWhenDueEstimate(List<Mileage> ms)
    {
        ServiceTaskRecord r = GetMostRecent();

        return Mileage.GetEstimatedDate(r.mileage+(int)mileageInterval,ms);
    }

}

public class ServiceTaskRecord{
    
    public string notes;

    public int mileage;

    public DateTime date;

}


public class OilChange:ServiceTask{

    public OilChange(){
        name = "Oil Change";
        description = "change the oil";
        repeat = Repeat.Repeat;
        mileageInterval = ServiceMileageInterval.num10000;
        timeinterval = ServiceTimeInterval.year1 ;

    }
}

public class OilFilterChange:ServiceTask{

    public OilFilterChange(){
        name = "Oil Filter Change";
        description = "change the oil filter";
        repeat = Repeat.Repeat;
        mileageInterval = ServiceMileageInterval.num10000;
        timeinterval = ServiceTimeInterval.year2;

        

    }
}

public class FuelFilterChange:ServiceTask{

    public FuelFilterChange(){
        name = "Fuel Filter Change";
        description = "change the fuel filter";
        repeat = Repeat.Repeat;
        mileageInterval = ServiceMileageInterval.num20000;
    }
}

public class DustFilter:ServiceTask{

    public DustFilter(){
        name = "Dust Filter Change";
        description = "dust filter change";
        repeat = Repeat.Repeat;
        mileageInterval = ServiceMileageInterval.num40000;

    }
}

public class PollenFilter:ServiceTask{

    public PollenFilter(){
        name = "Pollen Filter Change";
        description = "pollen filter change";
        repeat = Repeat.Repeat;
        mileageInterval = ServiceMileageInterval.num40000;

    }
}

public class ToothedBelt:ServiceTask{

    public ToothedBelt(){
        name = "Timing Belt Replace";
        description = "timing belt replace";
        repeat = Repeat.Repeat;
        mileageInterval = ServiceMileageInterval.num80000;

    }

}

public enum ServiceMileageInterval {na=0, num10000=10000,num20000=20000,num40000=40000,num80000=8000};
public enum ServiceTimeInterval {na=0, year1=1,year2=2,year3=3,year4=4};

public static class ServiceTaskLibrary{

    public static List<ServiceTask> servicetasks = new List<ServiceTask>();

    static ServiceTaskLibrary(){
        
        servicetasks.Add(new ToothedBelt());
        servicetasks.Add(new FuelFilterChange());
        servicetasks.Add(new PollenFilter());
        servicetasks.Add(new DustFilter());
        servicetasks.Add(new OilFilterChange());
        servicetasks.Add(new OilChange());
    }

}

public enum ServiceTaskStatus {overdue,due};


