library "jenkins-ptcs-library@3.1.0"

// pod provides common utilies and tools to jenkins-ptcs-library function correctly.
// certain ptcs-library command requires containers (like docker or gcloud.)
podTemplate(label: pod.label,
  containers: pod.templates + [ // This adds all depencies for jenkins-ptcs-library methods to function correctly.
    containerTemplate(name: 'dotnet5', image: 'mcr.microsoft.com/dotnet/sdk:5.0.100-alpine3.12', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    node(pod.label) {
      stage('Checkout') {
          checkout scm
      }
      stage('Build') {
        container('dotnet5') {
            sh """
                dotnet build
            """
        }
      }
      stage('Test') {
        container('dotnet5') {
            sh """
              dotnet test --framework=net5.0 Protacon.NetCore.WebApi.TestUtil.Tests
            """
        }
      }
      stage('Package') {
        container('dotnet5') {
          publishTagToNuget("Protacon.NetCore.WebApi.TestUtil")
        }
      }
    }
  }
