// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PhysicsBody.generated.h"


UCLASS(ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class ROCKETSIM_API UPhysicsBody : public UPrimitiveComponent
{
	GENERATED_BODY()

	
public:
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category=Any)
	float Mass = 0;

	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category=Any)
	FVector InitialVelocity = *new FVector(0,0,0);

private:
	bool InitialVelocitySet = false;
	
protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	
public:
	// Sets default values for this component's properties
	UPhysicsBody();
	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType,
	                           FActorComponentTickFunction* ThisTickFunction) override;
	
	FVector GetPosition();
	FVector GetVelocity();
};
