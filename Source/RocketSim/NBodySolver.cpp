﻿#include "NBodySolver.h"


NBodySolver::state_type NBodySolver::PackState(int dof, int n, int dim, TArray<UPhysicsBody*> & Bodies)
{
	state_type x = state_type(dof);

	if(dof < 1 )
		return x;

	//pack current state of bodies (positions and velocities) into x
	for(int i = 0; i < n; i++)
	{
		auto body = Bodies[i];
		auto pos = body->GetPosition();
		auto vel = body->GetVelocity();
		
		for(int j = 0; j < dim;j++)
		{
			x[dim*i + j] = pos[j];
			x[dim*i + j + dof/2] = vel[j];
		}
	}
	
	return x;
}

NBodySolver::return_type NBodySolver::UnpackState(int dim, int n, const state_type& x)
{
	return_type output;
	if(x.empty())
		return output;

	for(int i = 0; i < n; i++)
	{
		output.Add(UE::Math::TVector<double>(x[dim*i],x[dim*i+1],x[dim*i+2]));
	}

	return output;
}

NBodySolver::return_type NBodySolver::IntegrateOde(state_type& initial_state, float t0, float tf, float time_step,int dof,int n, int dim, double Gravity,TArray<UPhysicsBody*> &Bodies)
{

	state_type x = initial_state;

	integrate_const(stepper, ode{dof,n,dim,Gravity,Bodies},x, t0 , tf, time_step );
	// integrate_adaptive( stepper_type() , ode{dof,n,dim,Gravity,Bodies} , x , t0 , tf , time_step);
	return UnpackState(dim, n, x);
}



void NBodySolver::NBodyOde(state_type& x, state_type& dxdt, double t, int dof,int n, int dim, double Gravity,TArray<UPhysicsBody*> &Bodies)
{
	
	//return if number of bodies does not match dof / dim
	if(n != dof/(2*dim))
		return;

	//copy velocity from second half of x vector into first half of dx/dt vector
	for(int i = dof/2; i < x.size(); i++ )
	{
		auto index = i-dof/2;
		dxdt[index] = x[i];
	}
	
	for(int i = 0; i < n; i++)
	{
		//init accel for body i
		UE::Math::TVector<double> accel =  UE::Math::TVector<double>::ZeroVector;

		//position of body i at this instant given from x 
		UE::Math::TVector<double> r1 = UE::Math::TVector<double>(x[dim*i],x[dim*i+1],x[dim*i+2]);

		for(int j = 0; j < n; j++)
		{
			if(i!=j)
			{
				UE::Math::TVector<double> r2 = UE::Math::TVector<double>(x[dim*j],x[dim*j+1],x[dim*j+2]);
				const double m2 =  Bodies[j]->Mass;

				//accel on body i due to body j
				accel += -1 * (Gravity * m2)/std::pow((r1-r2).Length(),3) * (r1-r2);
			}
		}

		//copy accel values into second half of dx/dt vector
		for(int j = 0; j < dim; j++)
		{
			auto index = dof/2 + dim*i + j;
			dxdt[index] = accel[j];
		}
		
	}
}


// void NBodySolver::NBodyOdeSystem(state_type& x, state_type& v,state_type& a, double t)
// {
// 	if(n != dof/dim)
// 		return;
// 	
// 	for(int i = 0; i < n; i++)
// 	{
// 		//init accel for body i
// 		UE::Math::TVector<double> accel =  UE::Math::TVector<double>::ZeroVector;
// 		//get mass of body i
// 		double m1 = Bodies[i]->Mass;
// 		//position of body i at this instant given from x 
// 		UE::Math::TVector<double> r1 = UE::Math::TVector<double>(x[dim*i],x[dim*i+1],x[dim*i+2]);
//
// 		for(int j = 0; j < n; j++)
// 		{
// 			if(i!=j)
// 			{
// 				UE::Math::TVector<double> r2 = UE::Math::TVector<double>(x[dim*j],x[dim*j+1],x[dim*j+2]);
// 				double m2 = Bodies[j]->Mass;
//
// 				//accel on body i due to body j
// 				accel += -1 * (Gravity * m2)/std::pow((r1-r2).Length(),3) * (r1-r2);
// 			}
// 		}
//
// 		//copy accel values into second half of dx/dt vector
// 		for(int j = 0; j < dim; j++)
// 			a[dim*i + j] = accel[j];
// 	}
// 	
// }



