#include "NBodySolver.h"



NBodySolver::state_type NBodySolver::PackState()
{
	int const dim = ANBodyController::GetInstance()->Dim;
	int const n = ANBodyController::GetInstance()->NumBodies;
	int dof = 2*dim*n;

	state_type x = state_type(dof);

	//pack current state of bodies (positions and velocities) into x
	for(int i = 0; i < n; i++)
	{
		auto body = ANBodyController::GetInstance()->Bodies[i];
		auto pos = body->GetPosition();
		auto vel = body->GetVelocity();
		
		for(int j = 0; j < pos.Size();j++)
		{
			x[dim*i + j] = pos[j];
			x[dim*i + j + dof/2] = vel[j];
		}
	}
	
	return x;
}

NBodySolver::return_type NBodySolver::UnpackState(state_type& x)
{
	int const dim = ANBodyController::GetInstance()->Dim;
	int const n = ANBodyController::GetInstance()->NumBodies;
	return_type output;

	for(int i = 0; i < n; i++)
	{
		output.Add(UE::Math::TVector<double>(x[dim*i],x[dim*i+1],x[dim*i+2]));
	}

	return output;
}


NBodySolver::return_type NBodySolver::IntegrateOde(float t0, float tf, float time_step)
{
	state_type x = PackState();

	//integrate with predefined runga_kutta stepper, see header
	//state of system tf is returned in modified x = x(t=tf)
	integrate_const( stepper , NBodyOde , x , t0 , tf, time_step );

	return UnpackState(x);
	
}




void NBodySolver::NBodyOde(state_type& x, state_type& dxdt, double t)
{
	const int dof = x.size()/2; //degrees of freedom = half of x size due to x = [r,v]
	const int n = ANBodyController::GetInstance()->NumBodies; // get number of bodies in sim
	const int dim = ANBodyController::GetInstance()->Dim; //spacial dimension size

	//return if number of bodies does not match dof / dim
	if(n != dof/dim)
		return;

	//copy velocity from second half of x vector into first half of dx/dt vector
	for(int i = dof; i < x.size(); i++ )
		dxdt[i-dof] = x[i];
	
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
				const double m2 =  ANBodyController::GetInstance()->Bodies[j]->Mass;

				//accel on body i due to body j
				accel += -1 * (ANBodyController::GetInstance()->Gravity * m2)/std::pow((r1-r2).Length(),3) * (r1-r2);
			}
		}

		//copy accel values into second half of dx/dt vector
		for(int j = 0; j < dim; j++)
			dxdt[dof + dim*i + j] = accel[j];
	}
}


void NBodySolver::NBodyOdeSystem(state_type& x, state_type& v,state_type& a, double t)
{
	int dim = 3;
	int dof = x.size();
	int n = ANBodyController::GetInstance()->NumBodies;

	if(n != dof/dim)
		return;
	
	for(int i = 0; i < n; i++)
	{
		//init accel for body i
		UE::Math::TVector<double> accel =  UE::Math::TVector<double>::ZeroVector;
		//get mass of body i
		double m1 = ANBodyController::GetInstance()->Bodies[i]->Mass;
		//position of body i at this instant given from x 
		UE::Math::TVector<double> r1 = UE::Math::TVector<double>(x[dim*i],x[dim*i+1],x[dim*i+2]);

		for(int j = 0; j < n; j++)
		{
			if(i!=j)
			{
				UE::Math::TVector<double> r2 = UE::Math::TVector<double>(x[dim*j],x[dim*j+1],x[dim*j+2]);
				double m2 = ANBodyController::GetInstance()->Bodies[j]->Mass;

				//accel on body i due to body j
				accel += -1 * (ANBodyController::GetInstance()->Gravity * m2)/std::pow((r1-r2).Length(),3) * (r1-r2);
			}
		}

		//copy accel values into second half of dx/dt vector
		for(int j = 0; j < dim; j++)
			a[dim*i + j] = accel[j];
	}
	
}


