// Fill out your copyright notice in the Description page of Project Settings.


#include "NBodyController.h"

#include <string>

#include "CelestialBodyActor.h"
#include "PhysicsBody.h"
#include "Kismet/GameplayStatics.h"

ANBodyController* ANBodyController::GetInstance()
{
	if(Instance == nullptr)
	{
		ANBodyController temp = ANBodyController();
		Instance = &temp;
	}

	return Instance;
}

// Sets default values
ANBodyController::ANBodyController()
{
	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	Instance = this;
}

// Called when the game starts or when spawned
void ANBodyController::BeginPlay()
{
	TArray<AActor*> FoundActors;
	UGameplayStatics::GetAllActorsOfClass(GetWorld(), ACelestialBody::StaticClass(), FoundActors);
	for(int i = 0; i < FoundActors.Num();i++)
	{
		UE_LOG(LogTemp, Warning, TEXT("Found Celestial Body %d "),i);
		auto actor = FoundActors[i];
		auto b = Cast<UPhysicsBody,UActorComponent>(actor->GetComponentByClass(UPhysicsBody::StaticClass()));
		Bodies.Add(b);
	}

	NumBodies = Bodies.Num();
	
	Super::BeginPlay();
	
}

// Called every frame
void ANBodyController::Tick(float DeltaTime)
{
	int PhysicsIterations = (int) (DeltaTime/PhysicsTimeStep);
	
	if(SimMode== SimulationMode::Iterate)
	{
		for(int i = 0; i < PhysicsIterations; i++)
		{
			UpdateVelocityIterate(PhysicsTimeStep);
			UpdatePositionIterate(PhysicsTimeStep);
		}

		UE_LOG(LogTemp, Warning, TEXT("Completed Physics Iterations: %d"), PhysicsIterations);
	}

	if(SimMode== SimulationMode::Integrate)
	{
		auto t0 = UGameplayStatics::GetTimeSeconds(GetWorld());
		auto tf = t0 + DeltaTime;
		UpdatePositionIntegrate(t0,tf,PhysicsTimeStep);

		UE_LOG(LogTemp, Warning, TEXT("Completed Physics Integration from %f to %f"), t0,tf);
	}
	
	Super::Tick(DeltaTime);
}

void ANBodyController::UpdateVelocityIterate(float DeltaTime)
{
	for (int i = 0;i < NumBodies;i++)
	{
		auto body1 = Bodies[i];
		FVector accel = *new FVector(0,0,0);
		
		
		for(int j = 0; j < NumBodies; j++)
		{
			if(i != j)
			{
				auto body2 = Bodies[j];
				FVector ForceVec = body2->GetComponentLocation() - body1->GetComponentLocation();
				auto Dist = ForceVec.Length();
				ForceVec.Normalize();
				accel+= (Gravity*body2->Mass / (Dist*Dist))*ForceVec;
				UE_LOG(LogTemp, Warning, TEXT("Accel mag is %f"),accel.Length());
			}
		}

		body1->ComponentVelocity += DeltaTime*accel;
	}
}

void ANBodyController::UpdatePositionIterate(float DeltaTime)
{
	for (auto body: Bodies)
	{
		body->MoveComponent(DeltaTime*body->ComponentVelocity,body->GetComponentQuat(),false);
	}
}

void ANBodyController::UpdatePositionIntegrate(float t0, float tf, float DeltaTime)
{
	NBodySolver::return_type NextState = NBodySolver::IntegrateOde(t0,tf,DeltaTime);

	for(int i = 0; i < NumBodies; i++)
	{
		auto body = Bodies[i];
		auto pos = NextState[i];
		
		body->SetWorldLocation(pos);
	}
}






