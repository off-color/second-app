using System;
using covidSim.Models;

namespace covidSim.Services
{
    public class Person
    {
        private const int MaxDistancePerTurn = 20;
        private static Random random = new Random();
        private PersonState state = PersonState.AtHome;
        private int sickStepsCount = 0;
        private int deadStepsCount = 0;

        public Person(int id, int homeId, CityMap map, bool isSick)
        {
            Id = id;
            HomeId = homeId;
            if (isSick)
                PersonHealth = PersonHealth.Sick;

            var homeCoords = map.Houses[homeId].Coordinates.LeftTopCorner;
            var x = homeCoords.X + random.Next(HouseCoordinates.Width);
            var y = homeCoords.Y + random.Next(HouseCoordinates.Height);
            Position = new Vec(x, y);
        }

        private const int StepsToRecovery = 35;
        private const double ProbToDie = 0.000003;
        private const int StepsToDie = 10;
        public int Id;
        public int HomeId;
        public Vec Position;
        public PersonHealth PersonHealth = PersonHealth.Healthy;

        public void CalcNextStep()
        {
            if (PersonHealth == PersonHealth.Dead)
                return;

            if (PersonHealth == PersonHealth.Dying)
            {
                deadStepsCount++;
                if (deadStepsCount >= StepsToDie)
                    PersonHealth = PersonHealth.Dead;
                return;
            }

            switch (state)
            {
                case PersonState.AtHome:
                    CalcNextStepForPersonAtHome();
                    break;
                case PersonState.Walking:
                    CalcNextPositionForWalkingPerson();
                    break;
                case PersonState.GoingHome:
                    CalcNextPositionForGoingHomePerson();
                    break;
            }

            if (PersonHealth == PersonHealth.Sick)
            {
                if (random.NextDouble() <= ProbToDie)
                {
                    PersonHealth = PersonHealth.Dying;
                }
                sickStepsCount++;
                if (sickStepsCount >= StepsToRecovery)
                    PersonHealth = PersonHealth.Healthy;
            }
        }

        private void CalcNextStepForPersonAtHome()
        {
            var goingWalk = random.NextDouble() < 0.005;
            if (!goingWalk) return;

            state = PersonState.Walking;
            CalcNextPositionForWalkingPerson();
        }

        private void CalcNextPositionForWalkingPerson()
        {
            var xLength = random.Next(MaxDistancePerTurn);
            var yLength = MaxDistancePerTurn - xLength;
            var direction = ChooseDirection();
            var delta = new Vec(xLength * direction.X, yLength * direction.Y);
            var nextPosition = new Vec(Position.X + delta.X, Position.Y + delta.Y);

            if (isCoordInField(nextPosition))
            {
                Position = nextPosition;
            }
            else
            {
                CalcNextPositionForWalkingPerson();
            }
        }

        private void CalcNextPositionForGoingHomePerson()
        {
            var game = Game.Instance;
            var homeCoord = game.Map.Houses[HomeId].Coordinates.LeftTopCorner;
            var homeCenter = new Vec(homeCoord.X + HouseCoordinates.Width / 2, homeCoord.Y + HouseCoordinates.Height / 2);

            var xDiff = homeCenter.X - Position.X;
            var yDiff = homeCenter.Y - Position.Y;
            var xDistance = Math.Abs(xDiff);
            var yDistance = Math.Abs(yDiff);

            var distance = xDistance + yDistance;
            if (distance <= MaxDistancePerTurn)
            {
                Position = homeCenter;
                state = PersonState.AtHome;
                return;
            }

            var direction = new Vec(Math.Sign(xDiff), Math.Sign(yDiff));

            var xLength = Math.Min(xDistance, MaxDistancePerTurn);
            var newX = Position.X + xLength * direction.X;
            var yLength = MaxDistancePerTurn - xLength;
            var newY = Position.Y + yLength * direction.Y;
            Position = new Vec(newX, newY);
        }

        public void GoHome()
        {
            if (state != PersonState.Walking) return;

            state = PersonState.GoingHome;
            CalcNextPositionForGoingHomePerson();
        }

        private Vec ChooseDirection()
        {
            var directions = new Vec[]
            {
                new Vec(-1, -1),
                new Vec(-1, 1),
                new Vec(1, -1),
                new Vec(1, 1),
            };
            var index = random.Next(directions.Length);
            return directions[index];
        }

        private bool isCoordInField(Vec vec)
        {
            var belowZero = vec.X < 0 || vec.Y < 0;
            var beyondField = vec.X > Game.FieldWidth || vec.Y > Game.FieldHeight;

            return !(belowZero || beyondField);
        }
    }
}