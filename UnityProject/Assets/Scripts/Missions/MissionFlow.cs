using UnityEngine;
using UnityEngine.Events;

namespace Yemen4x4.Missions
{
    public enum GovernorateRoute { Mahweet, Aden, Taiz }
    public enum MissionState { AtDepot, LoadingPassengers, DrivingToDestination, UnloadingPassengers, ReturningToSanaa, Completed }

    public sealed class MissionFlow : MonoBehaviour
    {
        [SerializeField] private UnityEvent onPassengersLoaded;
        [SerializeField] private UnityEvent onPassengersUnloaded;
        [SerializeField] private UnityEvent<int> onRewardPaid;
        [SerializeField] private int mahweetReward = 1800, adenReward = 6500, taizReward = 3200;

        public GovernorateRoute SelectedRoute { get; private set; }
        public MissionState State { get; private set; } = MissionState.AtDepot;

        public void ChooseMahweet() => ChooseRoute(GovernorateRoute.Mahweet);
        public void ChooseAden() => ChooseRoute(GovernorateRoute.Aden);
        public void ChooseTaiz() => ChooseRoute(GovernorateRoute.Taiz);

        public void ChooseRoute(GovernorateRoute route)
        {
            if (State != MissionState.AtDepot && State != MissionState.Completed) return;
            SelectedRoute = route;
            State = MissionState.LoadingPassengers;
        }

        public void LoadPassengers()
        {
            if (State != MissionState.LoadingPassengers) return;
            State = MissionState.DrivingToDestination;
            onPassengersLoaded?.Invoke();
        }

        public void ArriveAtDestination()
        {
            if (State != MissionState.DrivingToDestination) return;
            State = MissionState.UnloadingPassengers;
            onPassengersUnloaded?.Invoke();
            int reward = SelectedRoute == GovernorateRoute.Mahweet ? mahweetReward :
                         SelectedRoute == GovernorateRoute.Aden ? adenReward : taizReward;
            onRewardPaid?.Invoke(reward);
            State = MissionState.ReturningToSanaa;
        }

        public void ReturnToDepot()
        {
            if (State != MissionState.ReturningToSanaa) return;
            State = MissionState.Completed;
        }

        public void StartNextMission() { if (State == MissionState.Completed) State = MissionState.AtDepot; }
    }
}
