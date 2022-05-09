// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "PhysicsEngine/RigidBodyBase.h"
#include "CelestialBodyActor.generated.h"

UCLASS()
class ROCKETSIM_API ACelestialBody : public ARigidBodyBase
{
	GENERATED_BODY()

public:
	// Sets default values for this actor's properties
	ACelestialBody();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;
};
