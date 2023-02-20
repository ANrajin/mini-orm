using System.Configuration;
using orm.console.Data;
using orm.console.Entities;

var connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

Course course1 = new Course()
{
    Id = 1,
    Title = "Full Asp.Net",
    Fees = 30000,
    Topics = new List<Topic>
    {
        new Topic
        {
            Id=1,
            Title = "Asp.net Core MVC Nuts & Bolts",
            Course_id= 1,
            Description = "A brief intro to asp.net and its feature",
            Sessions = new List<Session>
            {
                new Session
                {
                    Id = 1,
                    Topic_id= 1,
                    DurationInHour = 4,
                    LearningObjective = "Understand project types and structure"
                },
                new Session
                {
                    Id = 2,
                    Topic_id= 1,
                    DurationInHour = 4,
                    LearningObjective = "Model - View - Controller in depth"
                }
            }
        },
        new Topic
        {
            Id = 2,
            Title = "Understanding Razor",
            Course_id= 1,
            Description = "A brief discussion about razor pages and ragor syntexs",
            Sessions = new List<Session>
            {
                new Session
                {
                    Id = 3,
                    Topic_id= 2,
                    DurationInHour = 4,
                    LearningObjective = "Concept of layout and child page"
                },
                new Session
                {
                    Id = 4,
                    Topic_id= 2,
                    DurationInHour = 4,
                    LearningObjective = "Tag helpers & Html helpers"
                }
            }
        }
    },
    Tests = new List<AdmissionTest>
    {
        new AdmissionTest
        {
            Id = 1,
            Course_id = 1,
            StartDateTime = new DateTime(2021, 10, 05),
            EndDateTime = new DateTime(2021, 10, 10),
            TestFees = 100
        },
        new AdmissionTest
        {
            Id = 2,
            Course_id = 1,
            StartDateTime = new DateTime(2021, 09, 24),
            EndDateTime = new DateTime(2021, 09, 30),
            TestFees = 100
        }
    }
};

Course course2 = new Course()
{
    Id = 2,
    Title = "MERN Stack",
    Fees = 30000,
    Topics = new List<Topic>
    {
        new Topic
        {
            Id=3,
            Title = "React",
            Course_id= 2,
            Description = "Introduction to Controller",
            Sessions = new List<Session>
            {
                new Session
                {
                    Id = 5,
                    Topic_id= 3,
                    DurationInHour = 4,
                    LearningObjective = "How to work with contoller and resource"
                },
                new Session
                {
                    Id = 6,
                    Topic_id= 3,
                    DurationInHour = 4,
                    LearningObjective = "How to work with React"
                }
            }
        },
        new Topic
        {
            Id = 4,
            Title = "MongoDB",
            Course_id= 2,
            Description = "What is events",
            Sessions = new List<Session>
            {
                new Session
                {
                    Id = 7,
                    Topic_id= 4,
                    DurationInHour = 4,
                    LearningObjective = "how to send mail"
                },
                new Session
                {
                    Id = 8,
                    Topic_id= 4,
                    DurationInHour = 4,
                    LearningObjective = "Advance make service container"
                }
            }
        }
    },
    Tests = new List<AdmissionTest>
    {
        new AdmissionTest
        {
            Id = 3,
            Course_id = 2,
            StartDateTime = new DateTime(2021, 11, 06),
            EndDateTime = new DateTime(2021, 11, 10),
            TestFees = 100
        },
        new AdmissionTest
        {
            Id = 4,
            Course_id = 2,
            StartDateTime = new DateTime(2021, 12, 24),
            EndDateTime = new DateTime(2021, 12, 30),
            TestFees = 100
        }
    }
};

Instructor teacher = new Instructor()
{
    Name = "Jalal Uddin",
    Email = "jalaluddin@devskill.com",
    PresentAddress = new Address
    {
        Street = "100/A Mirpur",
        City = "Dhaka",
        Country = "Bangladesh"
    },
    PermanentAddress = new Address
    {
        Street = "100/A Mirpur",
        City = "Dhaka",
        Country = "Bangladesh"
    },
    PhoneNumbers = new List<Phone>
    {
        new Phone{Number = "123456789", Extension = "02", CountryCode = "+880"},
        new Phone{Number = "987456321", Extension = "02", CountryCode = "+880"}
    }
};


MyORM<Course> myORM = new MyORM<Course>(connectionStr);
//myORM.Insert(course1);
//myORM.Insert(course2);
//myORM.GetById(course1.Id);
//myORM.GetAll();
//myORM.Update(course1);
//myORM.Update(course2);
//myORM.Delete(course1);
//myORM.Delete(course2);
//myORM.Delete(1);