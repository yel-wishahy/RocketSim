// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PhysicsBody.h"
#include "GameFramework/Actor.h"
#include "NBodySolver.h"
#include "CelestialBodyActor.h"
#include "Kismet/GameplayStatics.h"
#include "NBodyController.generated.h"


UENUM()
enum class SimulationMode : uint8
{
	Iterate,
	Integrate
};

UCLASS()
class ROCKETSIM_API ANBodyController : public AActor
{
	GENERATED_BODY()

public:
	static ANBodyController* GetInstance();
	
	UPROPERTY(VisibleAnywhere,BlueprintReadWrite,Category=Any)
	TArray<UPhysicsBody*> Bodies;

	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category=Any)
	float PhysicsTimeStep = 0.01f;
	
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category=Any)
	SimulationMode SimMode = SimulationMode::Iterate;
	
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category=Any)
	/*Gravitation constant used in physics simulation (sim units are cm,s,kg*/
	double Gravity = 1.f;

	int NumBodies = 0;
	int Dim = 3;

private:
	NBodySolver::state_type InitialState;
	double LastPhysicsTime = 0;
	inline static ANBodyController* Instance;
	ANBodyController();
protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	//Update velocity of bodies with iterative method
	void UpdateVelocityIterate(float DeltaTime);

	//Update position of bodies with iterative method
	void UpdatePositionIterate(float DeltaTime);

	//Update position of bodies with ode integrate method
	void UpdatePositionIntegrate(float t0, float tf, float DeltaTime);

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	

	
};
