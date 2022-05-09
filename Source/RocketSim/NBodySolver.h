#pragma once

#include "NBodyController.h"

THIRD_PARTY_INCLUDES_START
#include "boost/numeric/odeint/stepper/runge_kutta4.hpp"
#include "boost/numeric/odeint/integrate/integrate_const.hpp"
THIRD_PARTY_INCLUDES_END

class NBodySolver
{
public:

	//state type used in ode to contain system state
	typedef std::vector<double> state_type;

	
	typedef TArray<UE::Math::TVector<double>> return_type;

	//solve N Body orbital trajectories based on current state of bodies
	//return TArray of TVector<double> elements containing positions of bodies at tf
	//Array is ordered the same as Bodies array
	static return_type IntegrateOde(float t0, float tf, float time_step);

private:
	//ode runga_kutta fourth order stepper
	inline static boost::numeric::odeint::runge_kutta4< state_type > stepper;

	//pack CURRENT positions and velocities of ANBodyController::Instance->Bodies into
	//the state_type: std::vector<double>
	static state_type PackState();

	//unpack state_type x into a TArray of TVector<double> elements
	static return_type UnpackState(state_type& x);
	
	//Ode for system of first order (x = [r,v] , dxdt=[v,a])
	static void NBodyOde(state_type& x, state_type& dxdt, double t);

	//Second order system prototype
	static void NBodyOdeSystem(state_type& x, state_type& v,state_type& a, double t);
	
};
