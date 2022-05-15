#pragma once

#include "PhysicsBody.h"
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

	//pack CURRENT positions and velocities of ANBodyController::Instance->Bodies into
	//the state_type: std::vector<double>
	static state_type PackState(int dof, int n, int dim, TArray<UPhysicsBody*> & Bodies);

	//unpack state_type x into a TArray of TVector<double> elements
	static return_type UnpackState(int dim, int n, const state_type& x);

	//solve N Body orbital trajectories based on current state of bodies
	//return TArray of TVector<double> elements containing positions of bodies at tf
	//Array is ordered the same as Bodies array
	
	static return_type IntegrateOde(state_type& initial_state, float t0, float tf, float time_step,int dof,int n, int dim, double Gravity,TArray<UPhysicsBody*> &Bodies);
	
	
private:
	//ode runga_kutta fourth order stepper with fixed time step
	inline static boost::numeric::odeint::runge_kutta4< state_type > stepper;
	
	// typedef boost::numeric::odeint::dense_output_runge_kutta<boost::numeric::odeint::controlled_runge_kutta<boost::numeric::odeint::runge_kutta_dopri5<state_type> > > stepper_type;

	
	//Ode for system of first order (x = [r,v] , dxdt=[v,a])
	static void NBodyOde(state_type& x, state_type& dxdt, double t,int dof,int n, int dim, double Gravity,TArray<UPhysicsBody*>& Bodies);

	//Second order system prototype //not used but keep for now as template
	// void NBodyOdeSystem(state_type& x, state_type& v,state_type& a, double t);

	// //struct to allow passing of additional parameters to odeint
	// //ONLY PASS THIS TO ODEINT, AS NBODYODE NEEDS ADDITIONAL INFO
	struct ode
	{
		int n;
		int dof;
		int dim;
		double gravity;
		TArray<UPhysicsBody*>& bodies;
		
		ode( int dof, int n, int dim,double gravity, TArray<UPhysicsBody*>& bodies ) : dof(dof), n(n), dim(dim),gravity(gravity),bodies(bodies) {}
	
		void operator()( state_type& x , state_type& dxdt , double t ) const 
		{
			NBodyOde(x,dxdt,t,dof,n,dim,gravity,bodies);
		}
	};
	
};
