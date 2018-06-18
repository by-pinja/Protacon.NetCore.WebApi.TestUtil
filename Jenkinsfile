// This file serves as working example and tests pipeline library functionality at same time.
// env.BRANCH_NAME isn't correct way to use this library elswhere.
// Actual version may be library 'jenkins-ptcs-library@2.0.0'
library "jenkins-ptcs-library@feature/nuget-publish"

// pod provides common utilies and tools to jenkins-ptcs-library function correctly.
// certain ptcs-library command requires containers (like docker or gcloud.)
podTemplate(label: pod.label,
  containers: pod.templates + [ // This adds all depencies for jenkins-ptcs-library methods to function correctly.
    containerTemplate(name: 'dotnet', image: 'microsoft/dotnet:2.1-sdk', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    node(pod.label) {
      stage('Checkout') {
          checkout scm
      }
      stage('Build') {
        container('dotnet') {
            sh """
                dotnet build
            """
        }
      }
      stage('Test') {
        container('dotnet') {
            sh """
                dotnet test
            """
        }
      }
      stage('Package') {
        container('dotnet') {
          publishTagToNuget("Protacon.NetCore.WebApi.TestUtil")
        }
      }
    }
  }
