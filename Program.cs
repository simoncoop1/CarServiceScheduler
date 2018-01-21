using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace CarServiceScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            //loadSerialized data
            Users.Instance().DeSerializeComplex();

            CliRun();

        }

        public static void CliRun()
        {
            PrintMenuWelcome();
            while(Menu1()== CLIMenuEnd.Fail){}
        }

        public static void PrintMenuWelcome(){
            Console.WriteLine("### Car Service Scheduler ### by Simon Cooper, version 0.001");
        }

        public static CLIMenuEnd Menu1()
        {

            Console.WriteLine("Menu:");
            Console.WriteLine("  1:Login");
            Console.WriteLine("  2:Create Account");
            Console.WriteLine("  3:Exit");

            String input = Console.ReadLine();
            if(input == "" || input == "1") {
                while(Menu2() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }    
            else if( input == "2"){
                while(Menu3() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "3"){

                Users.Instance().SerializeComplex();
                Console.WriteLine("Bye");
                return CLIMenuEnd.Success;
            }
            else
                return CLIMenuEnd.Fail;
        }

        public static CLIMenuEnd Menu2()
        {


            Console.WriteLine("Username:");
            String inputu = Console.ReadLine();

            Console.WriteLine("Password:");
            String inputp = Console.ReadLine();

            //Console.WriteLine("Username ");
            User u = Users.Instance().userAr.First(K => K.username == inputu);


            SHA512 sha512 = new SHA512CryptoServiceProvider();

            byte[] b = Encoding.ASCII.GetBytes(inputp);

            //check if user exists
            if(Users.Instance().userAr.Exists(k => k.username == inputu) &&
                u.password ==  Convert.ToBase64String(sha512.ComputeHash(b))){
                //log user in
                Session.Instance().user = u;
                while(Menu4() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Success;
                
            }
            else{
                Console.WriteLine();
                Console.WriteLine();
                CLI.WriteLineErrorMessage("Username or Password incorrect:");

                return CLIMenuEnd.Fail;
            }

        }

        public static CLIMenuEnd Menu3()
        {
            Console.WriteLine("Username:");
            String inputu = Console.ReadLine();

             //check if user exists
            if(Users.Instance().userAr.Exists(k => k.username == inputu)){
                Console.WriteLine("Username Exists");
            }

            Console.WriteLine("Password:");
            String inputp = Console.ReadLine();

            
            Console.WriteLine("First name:");
            String inputfn = Console.ReadLine();

            Console.WriteLine("Second name:");
            String inputsn = Console.ReadLine();

            int i = Users.Instance().userAr.Count;

            SHA512 sha512 = new SHA512CryptoServiceProvider();

            //Encoding.UTF8.GetBytes(inputp);
            byte[] b = Encoding.ASCII.GetBytes(inputp);
            //Console.WriteLine("Password hash:"+ Encoding.Default.GetString(sha512.ComputeHash(b)));

            User u = new User();
            u.username = inputu;
            u.firstname = inputfn;
            u.secondname = inputsn;
            //Encoding.Default.GetString();
            u.password = Convert.ToBase64String(sha512.ComputeHash(b));
            Users.Instance().userAr.Add(u);


            return CLIMenuEnd.Success;



        }

        public static CLIMenuEnd Menu4(){

            Console.WriteLine("Main Menu:");            
            Console.WriteLine("1. Select Car");
            Console.WriteLine("2. Add Car");
            Console.WriteLine("3. Logout");

            String input = Console.ReadLine();
            if(input == "" || input == "1") {
                while(Menu6() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }    
            else if( input == "2"){
                while(Menu7() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if( input == "3"){
                return CLIMenuEnd.Success;
            }
            else
                return CLIMenuEnd.Fail;
        }

        public static CLIMenuEnd Menu5(){

            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Record Mileage");
            Console.WriteLine("2. Show Recorded Mileage");            
            Console.WriteLine("3. Record Completed Service Item");
            Console.WriteLine("4. List Service Items");
            Console.WriteLine("5. Add Service Item");
            Console.WriteLine("6. List Service Items due soon or overdue");
            Console.WriteLine("7. Go to Car Select");

            Console.WriteLine("8. Edit Recorded Mileage");
            Console.WriteLine("9. Edit Recorded ServiceItem");


            Console.WriteLine("10. Exit");

            String input = Console.ReadLine();

            if(input == "1"){
                while(Menu8() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "2"){
                while(Menu9() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "3"){
                while(Menu13() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "4"){
                while(Menu14() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "5"){
                while(Menu12() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "10"){
                return CLIMenuEnd.Success;
            }
            else{
                return CLIMenuEnd.Fail;
            }
        }


        public static CLIMenuEnd Menu6(){

            Console.WriteLine("Select a car:");
            
            for(int i = 0; i < Session.Instance().user.cars.Count;i++){
                Console.WriteLine((i+1).ToString() + " " + Session.Instance().user.cars[i].model);
            }

            Console.WriteLine((1+Session.Instance().user.cars.Count).ToString() + " Exit");

            
            string input = Console.ReadLine();

            if(input == "1" && Session.Instance().user.cars.Count == 0){
                return CLIMenuEnd.Success;
            }
            else{
                Session.Instance().car = Session.Instance().user.cars[Int32.Parse(input)-1];
                while(Menu5() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Success;
            }

        }

        public static CLIMenuEnd Menu7(){
            Console.WriteLine("Add Car:");

            Console.WriteLine("Select Manufacturer:");

            for(int i = 0; i<ManufacturerLibrary.manufacturers.Count;i++){
                Console.WriteLine((i+1).ToString() + " " + ManufacturerLibrary.manufacturers[i].name.ToString());

            }

            string inputm = Console.ReadLine();
            
            Manufacturer m = null;
            if(Int32.Parse(inputm)>0 && Int32.Parse(inputm) < ManufacturerLibrary.manufacturers.Count){
                 m = ManufacturerLibrary.manufacturers[Int32.Parse(inputm)-1];

            }
            
            if(m == null){
                return CLIMenuEnd.Fail;
            }

            Console.WriteLine("Select Model:");
            for(int i = 0; i<m.models.Count;i++){
                Console.WriteLine((i+1).ToString() + " " + m.models[i].name.ToString());
            }

            string inputmo = Console.ReadLine();

            Model mo = null;

            if(Int32.Parse(inputmo)>0 && Int32.Parse(inputmo) < ManufacturerLibrary.manufacturers.Count){
                 mo = m.models[Int32.Parse(inputmo)-1];

            }

            if(mo == null){
                return CLIMenuEnd.Fail;
            }

            Console.WriteLine("Enter Number Plate:");
            string np = Console.ReadLine();

            Console.WriteLine("Enter Registration Date:");

            string rd = Console.ReadLine();
            DateTime dtrd = DateTime.Parse(rd);

            Console.WriteLine();
            CLI.WriteLineLabelColonTerminated("Manufacturer: "+m.name.ToString());
            CLI.WriteLineLabelColonTerminated(mo.ToString());
            CLI.WriteLineLabelColonTerminated("Number Plate: " + np);
            CLI.WriteLineLabelColonTerminated("Registration Date: " + dtrd.ToString());
            Console.WriteLine();
            Console.WriteLine("Is this information correct? (y)es/ (n)o/ (e)xit");


            string conf = Console.ReadLine();
            if(conf == "y"){
                //Car
                Car car = new Car();
                car.numberplate = np;
                car.manufacturer = m;
                car.model = mo;
                car.registrationdate = dtrd;

                Session.Instance().user.cars.Add(car);

                Console.WriteLine("Add Complete");

                return CLIMenuEnd.Success;

            }
            else if(conf == "n"){
                return CLIMenuEnd.Fail;
            }
            else{
                return CLIMenuEnd.Fail;
            }
            
        }

        public static CLIMenuEnd Menu8(){
            Console.WriteLine("Record Mileage:");
            Console.WriteLine("Enter date or keyword today");
            string inputdtm = Console.ReadLine();
            DateTime dtm;
            if(inputdtm == "today"){
                dtm = DateTime.Today;
            }
            dtm = DateTime.Parse(inputdtm);

            Console.WriteLine("Enter mileage");
            string inputm = Console.ReadLine();
            int mileage = Int32.Parse(inputm);

            Mileage m = new Mileage();
            m.mileage = mileage;
            m.date = dtm;

            Session.Instance().car.mileagerecording.Add(m);

            Console.WriteLine("Mileage added");

            return CLIMenuEnd.Success;

        }

        
        private static CLIMenuEnd Menu9()
        {
            Console.WriteLine("Recorded Mileages");

            Console.WriteLine("-----------------------------");
            foreach(Mileage m in Session.Instance().car.mileagerecording){
                Console.WriteLine(m.date.ToString() + "  " + m.mileage);
            }
            Console.WriteLine("-----------------------------");

            Console.WriteLine("1. Estimate mileage for a date");
            Console.WriteLine("2. Estimate date for a mileage");
            Console.WriteLine("3.Exit");

            string input = Console.ReadLine();

            if(input == "1"){
                while(Menu11() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "2"){
                while(Menu10() == CLIMenuEnd.Fail){}
                return CLIMenuEnd.Fail;
            }
            else if(input == "3"){
                return CLIMenuEnd.Success;
            }
            else{
                return CLIMenuEnd.Fail;
            }
        }

        private static CLIMenuEnd Menu10()
        {
            Console.WriteLine("Give a mileage to calculate the date for:");
            string inputm = Console.ReadLine();

            int m = Int32.Parse(inputm);

            DateTime dt = Mileage.GetEstimatedDate(m,Session.Instance().car.mileagerecording);

            Console.WriteLine("Extimated Date: " + dt.ToString());

            return CLIMenuEnd.Success;

        }

        private static CLIMenuEnd Menu11()
        {
            Console.WriteLine("Give a date to calculate the mileage for:");
            string inputm = Console.ReadLine();

            DateTime d = DateTime.Parse(inputm);

            int m = Mileage.GetEstimatedMileage(d,Session.Instance().car.mileagerecording);

            Console.WriteLine("Extimated Mileage: " + m);

            return CLIMenuEnd.Success;

        }

        private static CLIMenuEnd Menu12()
        {
            Console.WriteLine("Service Tasks:");
            for(int i = 0;i< ServiceTaskLibrary.servicetasks.Count;i++){
                Console.WriteLine((i+1).ToString() + " " +ServiceTaskLibrary.servicetasks[i].name);
            }

            string input = Console.ReadLine();

            ServiceTask t =  ServiceTaskLibrary.servicetasks[Int32.Parse(input)-1];

            Console.WriteLine("selected:" + t.name +" (e)dit or (c)ontinue");

            string inpute = Console.ReadLine();

            if(inpute == "c")
            {
                Session.Instance().car.servicetasks.Add(t);

            }
            else{
                return CLIMenuEnd.Fail;
            }


            return CLIMenuEnd.Success;

        }

        private static CLIMenuEnd Menu13()
        {
            Console.WriteLine("Record Completed Service Item");
            Console.WriteLine("Select service task");

            for(int i = 0;i< Session.Instance().car.servicetasks.Count;i++){
                Console.WriteLine((i+1).ToString() + " " +Session.Instance().car.servicetasks[i].name);
            }

            string input = Console.ReadLine();

            ServiceTask t =  Session.Instance().car.servicetasks[Int32.Parse(input)-1];

            Console.WriteLine("completion mileage:");

            string inputm = Console.ReadLine();

            ServiceTaskRecord str = new ServiceTaskRecord();
            str.mileage = Int32.Parse(inputm);

            Console.WriteLine("completion date, or keyword today:");

            string inputd = Console.ReadLine();

            str.date = DateTime.Parse(inputd);

            Console.WriteLine("completion notes");

            string inputn = Console.ReadLine();

            str.notes = inputn;

            t.servictaskrecord.Add(str);



            return CLIMenuEnd.Success;

        }

        private static CLIMenuEnd Menu14()
        {
            Console.WriteLine("List Service Tasks:");

            Car car = Session.Instance().car;

            for(int i = 0;i< car.servicetasks.Count;i++){
                Console.WriteLine((i+1).ToString() + " " + car.servicetasks[i].name);

                Mileage m = car.MostRecentMileage();

                DateTime? dtn = car.servicetasks[i].GetWhenDue(m);
                DateTime dt = dtn.Value;

                DateTime dte = car.servicetasks[i].GetWhenDueEstimate(car.mileagerecording);

                 Console.WriteLine("    " + "Due: " + dt.ToShortDateString() + 
                    "  Due(Mileage Est): " + dte.ToShortDateString());

            }

            int exit = car.servicetasks.Count;

            Console.WriteLine(exit.ToString() + ".Exit");

            string input = Console.ReadLine();

            if(input == exit.ToString()){
                return CLIMenuEnd.Success;
            }
            else{
                return CLIMenuEnd.Fail;
            }



            

        }


    }
}
