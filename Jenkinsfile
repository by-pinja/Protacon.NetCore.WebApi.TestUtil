library "jenkins-ptcs-library@2.2.0"

// pod provides common utilies and tools to jenkins-ptcs-library function correctly.
// certain ptcs-library command requires containers (like docker or gcloud.)
podTemplate(label: pod.label,
  containers: pod.templates + [ // This adds all depencies for jenkins-ptcs-library methods to function correctly.
    containerTemplate(name: 'dotnet21', image: 'microsoft/dotnet:2.1-sdk', ttyEnabled: true, command: '/bin/sh -c', args: 'cat'),
    containerTemplate(name: 'dotnet31', image: 'mcr.microsoft.com/dotnet/core/sdk:3.1', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    node(pod.label) {
      stage('Checkout') {
          checkout scm
      }
      stage('Build') {
        container('dotnet31') {
            sh """
                dotnet build
            """
        }
      }
      stage('Test') {
        // For some reason running 2.1 tests in CI requires combination of frameworks during test run,
        // otherwise it complains about invalid framework 3.1 even when --framework==...2.1 is set?
        container('dotnet31') {
            sh """
                dotnet test --framework=netcoreapp3.1 Protacon.NetCore.WebApi.TestUtil.Tests
            """
        }
      }
      stage('Package') {
        container('dotnet31') {
          publishTagToNuget("Protacon.NetCore.WebApi.TestUtil")
        }
      }
    }
  }
