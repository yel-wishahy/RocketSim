// Fill out your copyright notice in the Description page of Project Settings.


#include "PhysicsBody.h"
#include "GameFramework/MovementComponent.h"


// Sets default values for this component's properties
UPhysicsBody::UPhysicsBody()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;
	

	// ...
}


// Called when the game starts
void UPhysicsBody::BeginPlay()
{
	Super::BeginPlay();
	//set kinematic physics
	this->SetSimulatePhysics(false);
	this->ComponentVelocity += InitialVelocity;
	InitialVelocitySet = true;
	
}


// Called every frame
void UPhysicsBody::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
}

FVector UPhysicsBody::GetPosition()
{
	return GetComponentLocation();
}

FVector UPhysicsBody::GetVelocity()
{
	if(!InitialVelocitySet)
	{
		this->ComponentVelocity += InitialVelocity;
		InitialVelocitySet = true;
	}
		
	return this->ComponentVelocity;
}



